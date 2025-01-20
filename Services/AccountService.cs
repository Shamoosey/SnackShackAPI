﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SnackShackAPI.Database;
using SnackShackAPI.Database.Models;
using SnackShackAPI.DTOs;
using SnackShackAPI.Models;
using SnackShackAPI.Services;
using System.Runtime.InteropServices.Marshalling;

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

        public async Task<bool> UpdateAccountInformation(Guid acccountId, UpdateAccountInfomationRequest data)
        {
            bool result = false;
            try
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == acccountId);

                if (account == null)
                {
                    throw new Exception("Account doesn't exist");
                }

                account.AccountName = data.AccountName;

                result = (await _context.SaveChangesAsync()) > 0;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error occurred while updating account information {acccountId}");
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


        public async Task<List<AccountDTO>> GetAccountsByUser(Guid userId)
        {
            var result = new List<AccountDTO>();

            try
            {
                result = _context.Accounts.Where(x => x.UserId == userId)
                    .Include("Currency")
                    .Select(x => new AccountDTO {
                        AccountId = x.Id,
                        Amount = x.Amount,
                        AccountName = x.AccountName,
                        CurrencyCode = x.Currency.CurrencyCode,
                }).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error occurred while creating account for user {userId}");
            }

            return result;
        }
    }
    public interface IAccountService
    {
        Task<bool> CreateAccount(Guid userId, string name, decimal? startingAmount, string currencyCode);
        Task<List<AccountDTO>> GetAccountsByUser(Guid userId);

        Task<bool> UpdateAccountInformation(Guid acccountId, UpdateAccountInfomationRequest data);
        Task<bool> UpdateAccountBalance(UpdateAccountBalanceRequest request);
    }
}
