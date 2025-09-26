using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaChain.Data;
using PharmaChain.Models;
using PharmaChain.ViewModels;

namespace PharmaChain.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
            : base(userManager, context)
        {
        }

        public async Task<IActionResult> Index()
        {
            var dashboardData = new AdminDashboardViewModel
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalMedicines = await _context.Medicines.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),
                PendingApprovals = await _context.Users.CountAsync(u => !u.IsApproved),
                AdminUsers = await _context.Users.CountAsync(u => u.Role == "Admin"),
                ManufacturerUsers = await _context.Users.CountAsync(u => u.Role == "Manufacturer"),
                SupplierUsers = await _context.Users.CountAsync(u => u.Role == "Supplier"),
                CustomerUsers = await _context.Users.CountAsync(u => u.Role == "Customer"),
                // RecentUsers = await _context.Users
                //     .OrderByDescending(u => u.CreatedAt)
                //     .Take(10)
                //     .ToListAsync(),
                // RecentOrders = await _context.Orders
                //     .Include(o => o.Customer)
                //     .Include(o => o.Medicine)
                //     .OrderByDescending(o => o.OrderDate)
                //     .Take(5)
                //     .ToListAsync()
            };

            return View(dashboardData);
        }

        // User Management
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users
                .OrderBy(u => u.Role)
                .ThenBy(u => u.Name)
                .ToListAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Admin cannot edit themselves
            if (currentUser.Id == user.Id)
            {
                TempData["ErrorMessage"] = "You cannot edit your own account.";
                return RedirectToAction("Users");
            }

            // Get available roles (exclude Admin role for security)
            var availableRoles = new List<string> { "Manufacturer", "Supplier", "Customer" };

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                CompanyName = user.CompanyName,
                Email = user.Email ?? string.Empty,
                Role = user.Role,
                IsApproved = user.IsApproved,
                CreatedAt = user.CreatedAt,
                AvailableRoles = availableRoles,
                IsCurrentUser = false,
                CanChangeRole = true
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null) return Unauthorized();

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null) return NotFound();

                // Admin cannot edit themselves
                if (currentUser.Id == user.Id)
                {
                    TempData["ErrorMessage"] = "You cannot edit your own account.";
                    return RedirectToAction("Users");
                }

                // Prevent changing role to Admin
                if (model.Role == "Admin")
                {
                    ModelState.AddModelError("Role", "You cannot assign Admin role to any user.");
                    model.AvailableRoles = new List<string> { "Manufacturer", "Supplier", "Customer" };
                    return View(model);
                }

                // Store original role to check if it changed
                var originalRole = user.Role;

                user.Name = model.Role == "Customer" ? model.Name : null;
                user.CompanyName = model.Role != "Customer" ? model.CompanyName : null;
                user.Email = model.Email;
                user.UserName = model.Email;
                user.Role = model.Role;
                user.IsApproved = model.IsApproved;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    // Update role if changed
                    if (originalRole != model.Role)
                    {
                        var currentRoles = await _userManager.GetRolesAsync(user);
                        if (currentRoles.Any())
                        {
                            await _userManager.RemoveFromRolesAsync(user, currentRoles);
                        }
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }

                    TempData["SuccessMessage"] = "User updated successfully.";
                    return RedirectToAction("Users");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Re-populate available roles for the view
            model.AvailableRoles = new List<string> { "Manufacturer", "Supplier", "Customer" };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            user.IsApproved = true;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "User approved successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to approve user.";
            }

            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> RejectUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            user.IsApproved = false;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "User rejected successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to reject user.";
            }

            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser?.Id == userId)
            {
                TempData["ErrorMessage"] = "You cannot delete your own account.";
                return RedirectToAction("Users");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "User deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete user.";
            }

            return RedirectToAction("Users");
        }

        // Medicine Management
        public async Task<IActionResult> Medicines()
        {
            var medicines = await _context.Medicines
                .Include(m => m.Manufacturer)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
            return View(medicines);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMedicine(int id)
        {
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null) return NotFound();

            _context.Medicines.Remove(medicine);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Medicine deleted successfully.";
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
        public async Task<IActionResult> UpdateOrderStatus(int orderId, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return NotFound();

            order.Status = status;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Order status updated successfully.";
            return RedirectToAction("Orders");
        }

    }
}
