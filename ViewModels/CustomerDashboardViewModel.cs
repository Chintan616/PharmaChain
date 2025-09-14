using PharmaChain.Models;

namespace PharmaChain.ViewModels
{
    public class CustomerDashboardViewModel
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public List<Order> RecentOrders { get; set; } = new List<Order>();
    }
}
