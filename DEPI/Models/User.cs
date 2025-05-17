using System;

namespace UserAuthenticationAPI.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required string MembershipTier { get; set; }
    }
}