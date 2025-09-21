using System.ComponentModel.DataAnnotations;

namespace PharmaChain.ViewModels
{
    public class EditProfileViewModel
    {
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string? Name { get; set; }

        [StringLength(200)]
        [Display(Name = "Company Name")]
        public string? CompanyName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
    }
}
