using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAuthenticationAPI.Data;
using UserAuthenticationAPI.Models;
using UserAuthenticationAPI.Services.Interfaces;

namespace UserAuthenticationAPI.Services
{
    public class UserService : IUserService
    {
        private readonly InMemoryUserRepository _userRepository;

        public UserService(InMemoryUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<User> GetUserByIdAsync(Guid userId)
        {
            return Task.FromResult(_userRepository.GetUserById(userId));
        }

        public Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return Task.FromResult(_userRepository.GetAllUsers());
        }

        public Task<User> CreateUserAsync(User user)
        {
            if (user == null)
            {
                throw new InvalidOperationException("Cannot add a null user.");
            }
            _userRepository.AddUser(user);
            return Task.FromResult(user);
        }

        public Task UpdateUserAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Cannot update a null user");
            }
            
            // Check if the user exists first
            var existingUser = _userRepository.GetUserById(user.UserId);
            if (existingUser == null)
            {
                throw new KeyNotFoundException($"User with ID {user.UserId} not found");
            }
            
            // Update the user in the repository
            _userRepository.UpdateUser(user);
            
            return Task.CompletedTask;
        }

        public Task DeleteUserAsync(Guid userId)
        {
            // Implementation depends on how you want to handle deletion
            return Task.CompletedTask;
        }

        public Task<User> GetUserByUsernameAsync(string username)
        {
            var user = _userRepository.GetUserByUsername(username);
            // Don't throw exception, just return null
            return Task.FromResult(user);
        }

        public Task<User> GetUserByEmailAsync(string email)
        {
            var user = _userRepository.GetUserByEmail(email);
            // Don't throw exception, just return null
            return Task.FromResult(user);
        }
    }
}