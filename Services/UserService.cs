using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SnackShackAPI.Database;
using SnackShackAPI.Database.Models;
using SnackShackAPI.DTOs;

namespace SnackShackAPI.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly SnackShackContext _context;
        private readonly IMapper _mapper;

        public UserService(SnackShackContext context, IMapper mapper, ILogger<UserService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> CreateUser(string discordUserId, string email)
        {
            bool result = false;
            try
            {
                _context.Users.Add(new User {
                    DiscordUserID = discordUserId,
                    Email = email,
                    CreatedDate = DateTime.UtcNow,
                    IsAdmin = false,
                });
                result = (await _context.SaveChangesAsync() > 0);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while creating user");
                result = false;
            }
            return result;
        }

        public async Task<UserDTO> GetUserByDiscord(string discordUserId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.DiscordUserID == discordUserId);
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> GetUserById(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<IEnumerable<UserDTO>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }

        public async Task<bool> DeleteUser(Guid userId)
        {
            bool result = false;
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    throw new Exception("User doesn't exist");
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                result = true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while deleting user");
                result = false;
            }
            return result;
        }
    }

    public interface IUserService
    {
        Task<UserDTO> GetUserByDiscord(string discordUserId);
        Task<UserDTO> GetUserById(Guid userId);
        Task<IEnumerable<UserDTO>> GetUsers();
        Task<bool> CreateUser(string discordUserId, string email);
        Task<bool> DeleteUser(Guid userId);
    }
}
