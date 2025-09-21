using Microsoft.AspNetCore.Identity;

namespace PharmaChain.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; } // Optional - only for customers
        public string Role { get; set; } = string.Empty;
        public string? CompanyName { get; set; } // Required for Manufacturer, Supplier, Admin
        public bool IsApproved { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Display name based on role
        public string DisplayName => Role switch
        {
            "Customer" => Name ?? Email ?? "Unknown",
            "Manufacturer" or "Supplier" or "Admin" => CompanyName ?? Email ?? "Unknown",
            _ => Name ?? CompanyName ?? Email ?? "Unknown"
        };
    }
}
