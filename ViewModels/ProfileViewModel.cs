using System.ComponentModel.DataAnnotations;

namespace PharmaChain.ViewModels
{
    public class ProfileViewModel
    {
        [Display(Name = "Full Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;

        [Display(Name = "Status")]
        public bool IsApproved { get; set; }

        [Display(Name = "Member Since")]
        public DateTime CreatedAt { get; set; }
    }
}
