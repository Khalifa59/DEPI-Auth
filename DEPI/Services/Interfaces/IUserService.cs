using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserAuthenticationAPI.Models;

namespace UserAuthenticationAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(Guid userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(Guid userId);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByEmailAsync(string email);
    }
}