using System.ComponentModel.DataAnnotations;

namespace PharmaChain.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Company name cannot exceed 200 characters")]
        public string? CompanyName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = string.Empty;

        public bool IsApproved { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<string> AvailableRoles { get; set; } = new() { "Manufacturer", "Supplier", "Customer" };

        public bool IsCurrentUser { get; set; }

        public bool CanChangeRole { get; set; } = true;
    }
}
