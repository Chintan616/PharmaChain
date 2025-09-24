using System.ComponentModel.DataAnnotations;

namespace PharmaChain.ViewModels
{
    public class RegisterViewModel
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

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;

        public List<string> AvailableRoles { get; set; } = new() { "Manufacturer", "Supplier", "Customer" };
    }
}
