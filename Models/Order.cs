using System.ComponentModel.DataAnnotations;

namespace PharmaChain.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        
        [Required]
        public string CustomerID { get; set; } = string.Empty;
        
        [Required]
        public int MedicineID { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
        
        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        
        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        
        public decimal TotalAmount { get; set; }
        
        public ApplicationUser? Customer { get; set; }
        public Medicine? Medicine { get; set; }
        
        public DateTime? ApprovedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
    }
    
    public enum OrderStatus
    {
        Pending,        // Order placed by customer, waiting for approval
        Approved,       // Order approved by manufacturer/supplier
        Delivered,      // Order delivered to customer
        Rejected        // Order rejected by manufacturer/supplier
    }
}
