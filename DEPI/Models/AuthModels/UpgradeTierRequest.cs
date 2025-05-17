using System.ComponentModel.DataAnnotations;

namespace UserAuthenticationAPI.Models.AuthModels
{
    public class UpgradeTierRequest
    {
        [Required]
        public string MembershipTier { get; set; } = "Premium";
    }
}