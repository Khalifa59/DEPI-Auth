namespace UserAuthenticationAPI.Models.AuthModels
{
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? MembershipTier { get; set; }
    }
}