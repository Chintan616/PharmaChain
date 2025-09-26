using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaChain.Data;
using PharmaChain.Models;
using PharmaChain.ViewModels;

namespace PharmaChain.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : BaseController
    {
        public CustomerController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
            : base(userManager, context)
        {
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null) return NotFound();

            var dashboardData = new CustomerDashboardViewModel
            {
                TotalOrders = await _context.Orders
                    .Where(o => o.CustomerID == currentUser.Id)
                    .CountAsync(),
                PendingOrders = await _context.Orders
                    .Where(o => o.CustomerID == currentUser.Id && o.Status == OrderStatus.Pending)
                    .CountAsync(),
                DeliveredOrders = await _context.Orders
                    .Where(o => o.CustomerID == currentUser.Id && o.Status == OrderStatus.Delivered)
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

        // Search Medicines from Suppliers
        public async Task<IActionResult> SearchMedicines(string searchTerm = "")
        {
            // Get medicines available in supplier inventories along with supplier names
            var inventoryQuery = _context.Inventories
                .Where(i => i.Quantity > 0);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                inventoryQuery = inventoryQuery.Where(i =>
                    (i.Medicine != null && i.Medicine.Name.Contains(searchTerm)) ||
                    (i.Medicine != null && i.Medicine.BatchNo.Contains(searchTerm)));
            }

            // Materialize and group in memory to avoid EF translation issues with GroupBy on entities
            var inventoryItems = await inventoryQuery
                .Where(i => i.Medicine != null && i.User != null)
                .Select(i => new
                {
                    Medicine = i.Medicine!,
                    Manufacturer = i.Medicine!.Manufacturer,
                    Supplier = i.User!
                })
                .ToListAsync();

            var grouped = inventoryItems
                .GroupBy(x => x.Medicine.MedicineID)
                .Select(g => new MedicineSuppliersViewModel
                {
                    Medicine = g.Select(x => x.Medicine).First(),
                    SupplierNames = g
                        .Where(x => x.Supplier.Role == "Supplier")
                        .Select(x => x.Supplier.CompanyName ?? x.Supplier.Email ?? "Unknown")
                        .Distinct()
                        .OrderBy(name => name)
                        .ToList()
                })
                .OrderBy(r => r.Medicine.Name)
                .ToList();

            // Ensure manufacturer is loaded for each medicine
            foreach (var item in grouped)
            {
                if (item.Medicine.Manufacturer == null)
                {
                    var firstInventory = inventoryItems.First(x => x.Medicine.MedicineID == item.Medicine.MedicineID);
                    item.Medicine.Manufacturer = firstInventory.Manufacturer;
                }
            }

            var model = new SearchMedicinesViewModel
            {
                SearchTerm = searchTerm,
                Results = grouped
            };

            return View(model);
        }

        // Place Order with Supplier
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(int medicineId, int quantity)
        {
            var currentUser = await GetCurrentUserAsync();
            var medicine = await _context.Medicines.FindAsync(medicineId);

            if (medicine == null || quantity <= 0)
            {
                TempData["ErrorMessage"] = "Invalid medicine or quantity.";
                return RedirectToAction("SearchMedicines");
            }

            // Check if any supplier has this medicine in stock
            var availableInventory = await _context.Inventories
                .Where(i => i.MedicineID == medicineId && i.Quantity >= quantity)
                .FirstOrDefaultAsync();

            if (availableInventory == null)
            {
                TempData["ErrorMessage"] = "No supplier has sufficient stock of this medicine.";
                return RedirectToAction("SearchMedicines");
            }

            var order = new Order
            {
                CustomerID = currentUser!.Id,
                MedicineID = medicineId,
                Quantity = quantity,
                TotalAmount = medicine.Price * quantity,
                Status = OrderStatus.Pending
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Order placed successfully. Waiting for supplier to fulfill the order.";
            return RedirectToAction("Orders");
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

        // Order History
        public async Task<IActionResult> OrderHistory()
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