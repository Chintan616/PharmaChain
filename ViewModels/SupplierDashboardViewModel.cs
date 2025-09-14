using PharmaChain.Models;

namespace PharmaChain.ViewModels
{
    public class SupplierDashboardViewModel
    {
        public int TotalInventory { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public List<Order> RecentOrders { get; set; } = new List<Order>();
    }
}
