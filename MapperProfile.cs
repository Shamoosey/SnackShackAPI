using AutoMapper;
using SnackShackAPI.Database.Models;
using SnackShackAPI.DTOs;
using SnackShackAPI.Models;

namespace SnackShackAPI
{
    public class  MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<CurrencyExchangeRate, ExchangeRateDTO>().ReverseMap();
        }
    }
}
