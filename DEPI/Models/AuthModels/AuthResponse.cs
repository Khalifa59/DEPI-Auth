using UserAuthenticationAPI.Models;

namespace UserAuthenticationAPI.Models.AuthModels
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public User User { get; set; }
    }
}