using System.ComponentModel.DataAnnotations;

namespace PharmaChain.ViewModels
{
    public class MedicineViewModel
    {
        public int MedicineID { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Medicine Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Batch Number")]
        public string BatchNo { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive number")]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
    }
}
