using System.ComponentModel.DataAnnotations;

namespace UserAuthenticationAPI.Models.AuthModels
{
    public class LoginRequest
    {
        [Required]
        [Display(Name = "Username or Email")]
        public required string Username { get; set; } // Can be either username or email
        
        [Required]
        public required string Password { get; set; }
    }
}