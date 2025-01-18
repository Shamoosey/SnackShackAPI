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

        public async Task<bool> CreateUser(UserDTO createUserDTO)
        {
            bool result = false;
            try
            {
                // Map DTO to User entity
                var newUser = _mapper.Map<User>(createUserDTO);
                newUser.Id = Guid.NewGuid(); // Set a new GUID for the user
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                result = true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while creating user");
                result = false;
            }
            return result;
        }

        public async Task<bool> UpdateUser(Guid userId, UserDTO updateUserDTO)
        {
            bool result = false;
            try
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (existingUser == null)
                {
                    throw new Exception("User doesn't exist");
                }

                // Map DTO to existing User entity
                _mapper.Map(updateUserDTO, existingUser);
                await _context.SaveChangesAsync();
                result = true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while updating user");
                result = false;
            }
            return result;
        }

        public async Task<UserDTO> GetUser(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

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
        Task<UserDTO> GetUser(Guid userId);
        Task<IEnumerable<UserDTO>> GetUsers();
        Task<bool> UpdateUser(Guid userId, UserDTO updateUserDTO);
        Task<bool> CreateUser(UserDTO createUserDTO);
        Task<bool> DeleteUser(Guid userId);
    }
}
