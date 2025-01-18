using AutoMapper;
using SnackShackAPI.Database.Models;
using SnackShackAPI.DTOs;

namespace SnackShackAPI
{
    public class  MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>().ReverseMap();
        }
    }
}
