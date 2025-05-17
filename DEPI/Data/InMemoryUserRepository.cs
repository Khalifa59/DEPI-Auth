using System;
using System.Collections.Generic;
using System.Linq;
using UserAuthenticationAPI.Models;

namespace UserAuthenticationAPI.Data
{
    public class InMemoryUserRepository
    {
        private readonly List<User> _users = new List<User>();

        public void AddUser(User user)
        {
            _users.Add(user);
        }

        public User GetUserByUsername(string username)
        {
            return _users.FirstOrDefault(u => u.Username == username) ?? throw new InvalidOperationException("User not found.");
        }

        public User GetUserById(Guid userId)
        {
            return _users.FirstOrDefault(u => u.UserId == userId) ?? throw new InvalidOperationException("User not found.");
        }

        public User GetUserByEmail(string email)
        {
            Console.WriteLine($"Repository: Looking for user with email: '{email}'");
            
            // Print all users to debug
            Console.WriteLine($"Repository: Total users: {_users.Count}");
            foreach (var u in _users)
            {
                Console.WriteLine($"User: '{u.Username}', Email: '{u.Email}', " +
                    $"Email Match: {string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase)}");
            }

            // Use StringComparison.OrdinalIgnoreCase for case-insensitive comparison
            var user = _users.FirstOrDefault(u => 
                string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
                
            Console.WriteLine($"Repository: Email lookup result: {(user != null ? $"Found user '{user.Username}'" : "No user found")}");
            
            return user ?? throw new InvalidOperationException("User not found.");
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _users;
        }

        public void UpdateUser(User updatedUser)
        {
            var existingUserIndex = _users.FindIndex(u => u.UserId == updatedUser.UserId);
            if (existingUserIndex >= 0)
            {
                _users[existingUserIndex] = updatedUser;
            }
        }
    }
}