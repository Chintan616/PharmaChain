using PharmaChain.Models;

namespace PharmaChain.ViewModels
{
    public class ManufacturerDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalMedicines { get; set; }
        public int TotalOrders { get; set; }
        public int PendingApprovals { get; set; }
        public List<Order> RecentOrders { get; set; } = new List<Order>();
    }
}
