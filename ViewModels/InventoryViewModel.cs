using PharmaChain.Models;

namespace PharmaChain.ViewModels
{
    public class InventoryViewModel
    {
        public int InventoryID { get; set; }
        public int MedicineID { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}