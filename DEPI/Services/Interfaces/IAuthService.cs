using System.Threading.Tasks;
using UserAuthenticationAPI.Models.AuthModels;

namespace UserAuthenticationAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> Register(RegisterRequest request);
        Task<AuthResponse> Login(LoginRequest request);
    }
}