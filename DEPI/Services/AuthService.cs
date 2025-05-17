using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UserAuthenticationAPI.Models;
using UserAuthenticationAPI.Models.AuthModels;
using UserAuthenticationAPI.Services.Interfaces;

namespace UserAuthenticationAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;

        public AuthService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<AuthResponse> Register(RegisterRequest request)
        {
            // Add detailed logging
            Console.WriteLine($"AuthService: Registration for Username='{request.Username}', Email='{request.Email}'");
            
            // Check if username already exists
            var existingUserWithUsername = await _userService.GetUserByUsernameAsync(request.Username);
            if (existingUserWithUsername != null)
            {
                throw new InvalidOperationException("Username is already taken");
            }
            
            // Check if email already exists
            var existingUserWithEmail = await _userService.GetUserByEmailAsync(request.Email);
            if (existingUserWithEmail != null)
            {
                throw new InvalidOperationException("Email is already registered");
            }
            
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email, // Make sure email is being set
                PasswordHash = HashPassword(request.Password),
                MembershipTier = string.IsNullOrEmpty(request.MembershipTier) ? "Basic" : request.MembershipTier
            };
            
            Console.WriteLine($"AuthService: Created user object with Email='{user.Email}'");
            
            await _userService.CreateUserAsync(user);
            Console.WriteLine("AuthService: User added to repository");
            
            return new AuthResponse
            {
                Token = GenerateToken(user),
                User = user
            };
        }

        public async Task<AuthResponse> Login(LoginRequest request)
        {
            User user = null;
            
            try
            {
                // Check if input looks like an email
                bool isEmail = request.Username.Contains('@');
                
                if (isEmail)
                {
                    // Try email lookup first if it contains @
                    user = await _userService.GetUserByEmailAsync(request.Username);
                }
                else 
                {
                    // Try username lookup if it doesn't look like an email
                    user = await _userService.GetUserByUsernameAsync(request.Username);
                }
                
                // If email lookup failed, try username as fallback
                if (user == null && isEmail)
                {
                    user = await _userService.GetUserByUsernameAsync(request.Username);
                }
                
                // Check if user exists first
                if (user == null)
                {
                    // Use the same generic message for all auth failures
                    throw new UnauthorizedAccessException("Invalid username/email or password");
                }
                
                // Now check password only if user exists
                if (!VerifyPassword(request.Password, user.PasswordHash))
                {
                    throw new UnauthorizedAccessException("Invalid username/email or password");
                }
                
                return new AuthResponse
                {
                    Token = GenerateToken(user),
                    User = user
                };
            }
            catch (Exception ex) when (!(ex is UnauthorizedAccessException))
            {
                // Convert any other unexpected exceptions to the same generic auth failure message
                // This ensures consistent error messages
                throw new UnauthorizedAccessException("Invalid username/email or password");
            }
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }

        private string GenerateToken(User user)
        {
            // Token generation logic (e.g., JWT) would go here
            return "generated_token"; // Placeholder for generated token
        }
    }
}