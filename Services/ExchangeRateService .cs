using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SnackShackAPI.Database;
using SnackShackAPI.Models;

namespace SnackShackAPI.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly ILogger<ExchangeRateService> _logger;
        private readonly SnackShackContext _context;
        private readonly IMapper _mapper;

        public ExchangeRateService(SnackShackContext context, IMapper mapper, ILogger<ExchangeRateService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ExchangeRateDTO>> GetExchangeRates()
        {
            var exchangeRates = await _context.CurrencyExchangeRates.Select(x => new ExchangeRateDTO
            {
                FromCurrencyId = x.FromCurrencyId,
                ToCurrencyId = x.ToCurrencyId,
                Rate = x.Rate
            }).ToListAsync();
            return exchangeRates;
        }

    }
    public interface IExchangeRateService
    {
        Task<List<ExchangeRateDTO>> GetExchangeRates();
    }
}
