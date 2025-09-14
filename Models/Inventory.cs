using System.ComponentModel.DataAnnotations;

namespace PharmaChain.Models
{
    public class Inventory
    {
        public int InventoryID { get; set; }
        
        [Required]
        public string UserID { get; set; } = string.Empty;
        
        [Required]
        public int MedicineID { get; set; }
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive number")]
        public int Quantity { get; set; }
        
        public ApplicationUser? User { get; set; }
        public Medicine? Medicine { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
