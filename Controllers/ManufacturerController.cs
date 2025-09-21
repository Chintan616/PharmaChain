using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaChain.Data;
using PharmaChain.Models;
using PharmaChain.ViewModels;

namespace PharmaChain.Controllers
{
    [Authorize(Roles = "Manufacturer")]
    public class ManufacturerController : BaseController
    {
        public ManufacturerController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
            : base(userManager, context)
        {
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null) return NotFound();

            var dashboardData = new ManufacturerDashboardViewModel
            {
                TotalMedicines = await _context.Medicines.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),
                PendingApprovals = await _context.Users.CountAsync(u => !u.IsApproved),
                LowStockMedicines = await _context.Medicines.CountAsync(m => m.Quantity <= Medicine.LOW_STOCK_THRESHOLD),
                RecentOrders = await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Medicine)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .ToListAsync(),
                LowStockItems = await _context.Medicines
                    .Where(m => m.Quantity <= Medicine.LOW_STOCK_THRESHOLD)
                    .OrderBy(m => m.Quantity)
                    .Take(5)
                    .ToListAsync()
            };

            return View(dashboardData);
        }

        // User Management
        public async Task<IActionResult> Users()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null) return NotFound();

            // Get customers who have ordered medicines from this manufacturer
            var customerIds = await _context.Orders
                .Where(o => o.Medicine.ManufacturerID == currentUser.Id)
                .Select(o => o.CustomerID)
                .Distinct()
                .ToListAsync();

            // Get suppliers who have inventory of medicines from this manufacturer
            var supplierIds = await _context.Inventories
                .Where(i => i.Medicine.ManufacturerID == currentUser.Id)
                .Select(i => i.UserID)
                .Distinct()
                .ToListAsync();

            // Combine both lists and get unique users
            var allRelevantUserIds = customerIds.Union(supplierIds).ToList();

            // Get users who are either customers or suppliers and are relevant to this manufacturer
            var users = await _context.Users
                .Where(u => allRelevantUserIds.Contains(u.Id) && 
                           (u.Role == "Customer" || u.Role == "Supplier"))
                .OrderBy(u => u.Role)
                .ThenBy(u => u.CreatedAt)
                .ToListAsync();

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveUser(string userId)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null) return NotFound();

            // Verify the user is related to this manufacturer's business
            var isRelatedUser = await IsUserRelatedToManufacturer(userId, currentUser.Id);
            if (!isRelatedUser)
            {
                TempData["ErrorMessage"] = "You can only manage users related to your medicines.";
                return RedirectToAction("Users");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.IsApproved = true;
                await _userManager.UpdateAsync(user);
                TempData["SuccessMessage"] = "User approved successfully.";
            }
            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> RejectUser(string userId)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null) return NotFound();

            // Verify the user is related to this manufacturer's business
            var isRelatedUser = await IsUserRelatedToManufacturer(userId, currentUser.Id);
            if (!isRelatedUser)
            {
                TempData["ErrorMessage"] = "You can only manage users related to your medicines.";
                return RedirectToAction("Users");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
                TempData["SuccessMessage"] = "User rejected and deleted.";
            }
            return RedirectToAction("Users");
        }

        private async Task<bool> IsUserRelatedToManufacturer(string userId, string manufacturerId)
        {
            // Check if user is a customer who ordered medicines from this manufacturer
            var isCustomer = await _context.Orders
                .AnyAsync(o => o.CustomerID == userId && o.Medicine.ManufacturerID == manufacturerId);

            // Check if user is a supplier who has inventory of medicines from this manufacturer
            var isSupplier = await _context.Inventories
                .AnyAsync(i => i.UserID == userId && i.Medicine.ManufacturerID == manufacturerId);

            return isCustomer || isSupplier;
        }

        // Medicine Management
        public async Task<IActionResult> Medicines()
        {
            var currentUser = await GetCurrentUserAsync();
            var medicines = await _context.Medicines
                .Where(m => m.ManufacturerID == currentUser!.Id)
                .ToListAsync();
            return View(medicines);
        }

        [HttpGet]
        public IActionResult CreateMedicine()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateMedicine(MedicineViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await GetCurrentUserAsync();
                var medicine = new Medicine
                {
                    Name = model.Name,
                    BatchNo = model.BatchNo,
                    ExpiryDate = model.ExpiryDate,
                    Quantity = model.Quantity,
                    Price = model.Price,
                    ManufacturerID = currentUser!.Id
                };

                _context.Medicines.Add(medicine);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Medicine created successfully.";
                return RedirectToAction("Medicines");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditMedicine(int id)
        {
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null) return NotFound();

            var model = new MedicineViewModel
            {
                MedicineID = medicine.MedicineID,
                Name = medicine.Name,
                BatchNo = medicine.BatchNo,
                ExpiryDate = medicine.ExpiryDate,
                Quantity = medicine.Quantity,
                Price = medicine.Price
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditMedicine(MedicineViewModel model)
        {
            if (ModelState.IsValid)
            {
                var medicine = await _context.Medicines.FindAsync(model.MedicineID);
                if (medicine == null) return NotFound();

                medicine.Name = model.Name;
                medicine.BatchNo = model.BatchNo;
                medicine.ExpiryDate = model.ExpiryDate;
                medicine.Quantity = model.Quantity;
                medicine.Price = model.Price;
                medicine.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Medicine updated successfully.";
                return RedirectToAction("Medicines");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMedicine(int id)
        {
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine != null)
            {
                _context.Medicines.Remove(medicine);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Medicine deleted successfully.";
            }
            return RedirectToAction("Medicines");
        }

        // Order Management
        public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Medicine)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveOrder(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = OrderStatus.Approved;
                order.ApprovedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Order approved successfully.";
            }
            return RedirectToAction("Orders");
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
            return RedirectToAction("Orders");
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
            return RedirectToAction("Orders");
        }
    }
}
