using PharmaChain.Models;

namespace PharmaChain.ViewModels
{
    public class SupplierDashboardViewModel
    {
        public int TotalInventory { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int LowStockItems { get; set; }
        public List<Order> RecentOrders { get; set; } = new List<Order>();
        public List<Inventory> LowStockInventory { get; set; } = new List<Inventory>();
    }
}
