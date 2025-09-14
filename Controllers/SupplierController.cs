using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaChain.Data;
using PharmaChain.Models;
using PharmaChain.ViewModels;

namespace PharmaChain.Controllers
{
    [Authorize(Roles = "Supplier")]
    public class SupplierController : BaseController
    {
        public SupplierController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
            : base(userManager, context)
        {
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null) return NotFound();

            var dashboardData = new SupplierDashboardViewModel
            {
                TotalInventory = await _context.Inventories
                    .Where(i => i.UserID == currentUser.Id)
                    .SumAsync(i => i.Quantity),
                TotalOrders = await _context.Orders
                    .Where(o => o.CustomerID == currentUser.Id)
                    .CountAsync(),
                PendingOrders = await _context.Orders
                    .Where(o => o.CustomerID == currentUser.Id && o.Status == OrderStatus.Pending)
                    .CountAsync(),
                RecentOrders = await _context.Orders
                    .Where(o => o.CustomerID == currentUser.Id)
                    .Include(o => o.Medicine)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .ToListAsync()
            };

            return View(dashboardData);
        }

        // Inventory Management
        public async Task<IActionResult> Inventory()
        {
            var currentUser = await GetCurrentUserAsync();
            var inventory = await _context.Inventories
                .Where(i => i.UserID == currentUser!.Id)
                .Include(i => i.Medicine)
                .ToListAsync();
            return View(inventory);
        }

        // Buy from Manufacturer
        public async Task<IActionResult> BuyFromManufacturer()
        {
            var medicines = await _context.Medicines
                .Include(m => m.Manufacturer)
                .ToListAsync();
            return View(medicines);
        }

        [HttpPost]
        public async Task<IActionResult> PurchaseMedicine(int medicineId, int quantity)
        {
            var currentUser = await GetCurrentUserAsync();
            var medicine = await _context.Medicines.FindAsync(medicineId);

            if (medicine == null || quantity <= 0)
            {
                TempData["ErrorMessage"] = "Invalid medicine or quantity.";
                return RedirectToAction("BuyFromManufacturer");
            }

            if (medicine.Quantity < quantity)
            {
                TempData["ErrorMessage"] = "Insufficient stock available.";
                return RedirectToAction("BuyFromManufacturer");
            }

            // Update medicine quantity
            medicine.Quantity -= quantity;
            medicine.UpdatedAt = DateTime.UtcNow;

            // Add to supplier inventory
            var existingInventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.UserID == currentUser!.Id && i.MedicineID == medicineId);

            if (existingInventory != null)
            {
                existingInventory.Quantity += quantity;
                existingInventory.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                var inventory = new Inventory
                {
                    UserID = currentUser!.Id,
                    MedicineID = medicineId,
                    Quantity = quantity
                };
                _context.Inventories.Add(inventory);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Medicine purchased successfully.";
            return RedirectToAction("Inventory");
        }

        // View Orders from Customers
        public async Task<IActionResult> CustomerOrders()
        {
            var currentUser = await GetCurrentUserAsync();
            
            // Get medicine IDs that this supplier has in inventory
            var supplierMedicineIds = await _context.Inventories
                .Where(i => i.UserID == currentUser!.Id)
                .Select(i => i.MedicineID)
                .ToListAsync();

            var orders = await _context.Orders
                .Where(o => supplierMedicineIds.Contains(o.MedicineID))
                .Include(o => o.Customer)
                .Include(o => o.Medicine)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveOrder(int orderId)
        {
            var currentUser = await GetCurrentUserAsync();
            var order = await _context.Orders
                .Include(o => o.Medicine)
                .FirstOrDefaultAsync(o => o.OrderID == orderId);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction("CustomerOrders");
            }

            // Check if supplier has this medicine in inventory
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.UserID == currentUser!.Id && i.MedicineID == order.MedicineID);

            if (inventory == null || inventory.Quantity < order.Quantity)
            {
                TempData["ErrorMessage"] = "Insufficient inventory to fulfill this order.";
                return RedirectToAction("CustomerOrders");
            }

            // Reduce inventory
            inventory.Quantity -= order.Quantity;
            inventory.UpdatedAt = DateTime.UtcNow;

            // Update order status
            order.Status = OrderStatus.Approved;
            order.ApprovedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Order approved and inventory updated successfully.";
            return RedirectToAction("CustomerOrders");
        }

        [HttpPost]
        public async Task<IActionResult> RejectOrder(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = OrderStatus.Rejected;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Order rejected successfully.";
            }
            return RedirectToAction("CustomerOrders");
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsDelivered(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = OrderStatus.Delivered;
                order.DeliveredAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Order marked as delivered successfully.";
            }
            return RedirectToAction("CustomerOrders");
        }

        // View Orders
        public async Task<IActionResult> Orders()
        {
            var currentUser = await GetCurrentUserAsync();
            var orders = await _context.Orders
                .Where(o => o.CustomerID == currentUser!.Id)
                .Include(o => o.Medicine)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);
        }
    }
}
