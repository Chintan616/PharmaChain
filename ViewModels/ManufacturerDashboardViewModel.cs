using PharmaChain.Models;

namespace PharmaChain.ViewModels
{
    public class ManufacturerDashboardViewModel
    {
        public int TotalMedicines { get; set; }
        public int TotalOrders { get; set; }
        public int PendingApprovals { get; set; }
        public int LowStockMedicines { get; set; }
        public List<Order> RecentOrders { get; set; } = new List<Order>();
        public List<Medicine> LowStockItems { get; set; } = new List<Medicine>();
    }
}
