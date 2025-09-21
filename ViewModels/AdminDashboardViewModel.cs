using PharmaChain.Models;

namespace PharmaChain.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalMedicines { get; set; }
        public int TotalOrders { get; set; }
        public int PendingApprovals { get; set; }
        public int AdminUsers { get; set; }
        public int ManufacturerUsers { get; set; }
        public int SupplierUsers { get; set; }
        public int CustomerUsers { get; set; }
        public List<ApplicationUser> RecentUsers { get; set; } = new();
        public List<Order> RecentOrders { get; set; } = new();
    }
}


