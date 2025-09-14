using System.ComponentModel.DataAnnotations;

namespace PharmaChain.ViewModels
{
    public class EditProfileViewModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
    }
}
