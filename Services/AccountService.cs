using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SnackShackAPI.Database;
using SnackShackAPI.Database.Models;
using SnackShackAPI.DTOs;
using SnackShackAPI.Models;
using SnackShackAPI.Models.DTOs;

namespace SnackShackAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly ILogger<UserService> _logger;
        private readonly SnackShackContext _context;
        private readonly IMapper _mapper;

        public AccountService(SnackShackContext context, IMapper mapper, ILogger<UserService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> CreateAccount(Guid userId, string name, decimal? startingAmount, string currencyCode)
        {
            bool result = false;
            try
            {
                var currency = await this._context.Currencies.FirstOrDefaultAsync(x => x.CurrencyCode == currencyCode);

                if (currency == null)
                {
                    throw new Exception("Currency doesn't exist");
                }

                await this._context.Accounts.AddAsync(new Account
                {
                    Amount = startingAmount ?? 0,
                    CreatedDate = DateTime.UtcNow,
                    UserId = userId,
                    Currency = currency,
                    AccountName = name,
                });

                await _context.SaveChangesAsync();
                result = true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error occurred while creating account for user {userId}");
                result = false;
            }
            return result;
        }

        public async Task<bool> UpdateAccountBalance(UpdateAccountBalanceRequest request)
        {
            bool result = false;
            try
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(x => x.UserId == request.UserId && x.Id == request.AccountId);
                if (account == null)
                {
                    throw new Exception("Account doesn't exist");
                }
                
                var oldBalance = account.Amount;
                var newBalance = oldBalance + request.Amount;
                account.Amount = newBalance;

                Guid? receieverAccountId =
                    request.TransactionType == TransactionType.BankToAccount ?
                    request.AccountId : null;

                Guid? senderAccountId =
                    request.TransactionType == TransactionType.AccountToBank ?
                    request.AccountId : null;

                var transaction = new Transaction
                {
                    TransactionDate = DateTime.UtcNow,
                    TransactionType = request.TransactionType,
                    Amount = request.Amount,
                    InitiatedByUserId = request.UserId,
                    ReceiverAccountId = receieverAccountId,
                    SenderAccountId = senderAccountId,
                    Notes = request.Notes
                };
                _context.Transactions.Add(transaction);

                _context.AccountHistories.Add(new AccountHistory
                {
                    AccountId = request.AccountId,
                    ChangeDate = DateTime.UtcNow,
                    NewAmount = newBalance,
                    PreviousAmount = oldBalance,
                    TransactionId = transaction.Id,
                });

                result = (await _context.SaveChangesAsync()) > 0;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error occurred while updateing account balance for user {request.UserId}");
                result = false;
            }

            return result;
        }


        public async Task<List<AccountDTO>> GetAccountsByDiscordUser(string discordUserId)
        {
            var result = new List<AccountDTO>();

            try
            {
                result = _context.Accounts
                    .Include("User")
                    .Include("Currency")
                    .Where(x => x.User.DiscordUserID == discordUserId)
                    .Select(x => new AccountDTO {
                        AccountId = x.Id,
                        Amount = x.Amount,
                        AccountName = x.AccountName,
                        CurrencyId = x.Currency.Id,
                        CurrencyCode = x.Currency.CurrencyCode,
                }).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error occurred while creating account for user {discordUserId}");
            }

            return result;
        }

        public async Task<bool> TransferFunds(TransferFundsRequest request)
        {
            var result = false;
            var transaction = _context.Database.BeginTransaction();

            try
            {
                var senderAccount = _context.Accounts.Include("Currency").FirstOrDefault(x => x.Id == request.FromAccountId);
                var receiverAccount = _context.Accounts.Include("Currency").FirstOrDefault(x => x.Id == request.ToAccountId);

                if (senderAccount == null)
                {
                    throw new Exception("Sender Account doesn't exist");
                }

                if (receiverAccount == null)
                {
                    throw new Exception("Receiver Account doesn't exist");
                }

                if (senderAccount.Amount < request.Amount)
                {
                    throw new Exception("Insufficient funds, unable to complete request");
                }

                var toCurrencyExchangeRate = _context.CurrencyExchangeRates.FirstOrDefault(x =>
                    x.ToCurrencyId == receiverAccount.CurrencyId &&
                    x.FromCurrencyId == senderAccount.CurrencyId
                );
                decimal newConvertedAmount = (decimal)((double)request.Amount * (toCurrencyExchangeRate?.Rate ?? 1));

                var accountTransaction = new Transaction
                {
                    TransactionDate = DateTime.UtcNow,
                    TransactionType = TransactionType.AccountToAccount,
                    Amount = request.Amount,
                    InitiatedByUserId = request.UserId,
                    ReceiverAccountId = request.ToAccountId,
                    SenderAccountId = request.FromAccountId,
                    Notes = $"Account transfer {request.Amount} {senderAccount.Currency.CurrencyCode} => {newConvertedAmount} {receiverAccount.Currency.CurrencyCode}"
                };
                _context.Transactions.Add(accountTransaction);


                var senderOldBalance = senderAccount.Amount;
                var senderNewBalance = senderOldBalance - request.Amount;
                senderAccount.Amount = senderNewBalance;

                _context.AccountHistories.Add(new AccountHistory
                {
                    AccountId = request.FromAccountId,
                    ChangeDate = DateTime.UtcNow,
                    NewAmount = senderNewBalance,
                    PreviousAmount = senderOldBalance,
                    TransactionId = accountTransaction.Id,
                });

                var receiverOldBalance = receiverAccount.Amount;
                var receiverNewBalance = receiverOldBalance + newConvertedAmount;
                receiverAccount.Amount = receiverNewBalance;
                
                _context.AccountHistories.Add(new AccountHistory
                {
                    AccountId = request.ToAccountId,
                    ChangeDate = DateTime.UtcNow,
                    NewAmount = receiverNewBalance,
                    PreviousAmount = receiverOldBalance,
                    TransactionId = accountTransaction.Id,
                });

                result = (await _context.SaveChangesAsync()) > 0;
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                _logger.LogError(e, $"Error occurred while transfering funds for user {request.UserId}");
                result = false;
            }

            return result;
        }

        public async Task<List<AccountHistoryDTO>> GetAccountHistory(string userId, Guid accountId)
        {
            var result = new List<AccountHistoryDTO>();

            try
            {
                result = await (
                    from ah in _context.AccountHistories
                    join t in _context.Transactions
                    on ah.TransactionId equals t.Id
                    join a in _context.Accounts
                    on ah.AccountId equals a.Id
                    join u in _context.Users
                    on a.UserId equals u.Id
                    join c in _context.Currencies
                    on a.CurrencyId equals c.Id
                    where u.DiscordUserID == userId && a.Id == accountId
                    select new AccountHistoryDTO
                    {
                        ChangeDate = ah.ChangeDate,
                        NewAmount = ah.NewAmount,
                        PreviousAmount = ah.PreviousAmount,
                        TransactionAmount = t.Amount,
                        TransactionNotes = t.Notes ?? "",
                        CurrencyCode = c.CurrencyCode
                    }
                ).OrderByDescending(x => x.ChangeDate).ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error occurred while getting account histories for user {userId}");
            }

            return result;
        }
    }
    public interface IAccountService
    {
        Task<bool> CreateAccount(Guid userId, string name, decimal? startingAmount, string currencyCode);
        Task<List<AccountDTO>> GetAccountsByDiscordUser(string discordUserId);
        Task<List<AccountHistoryDTO>> GetAccountHistory(string discordUserId, Guid account);
        Task<bool> UpdateAccountBalance(UpdateAccountBalanceRequest request);
        Task<bool> TransferFunds(TransferFundsRequest request);
    }
}
