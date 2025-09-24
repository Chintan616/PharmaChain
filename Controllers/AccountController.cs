using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PharmaChain.Data;
using PharmaChain.Models;
using PharmaChain.ViewModels;

namespace PharmaChain.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Prevent admin registration
            if (model.Role == "Admin")
            {
                ModelState.AddModelError("Role", "Admin registration is not allowed. Admin accounts can only be created by system administrators.");
                model.AvailableRoles = new List<string> { "Manufacturer", "Supplier", "Customer" };
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Role == "Customer" ? model.Name : null,
                    CompanyName = model.Role != "Customer" ? model.CompanyName : null,
                    Role = model.Role,
                    // Approval rules: Manufacturer requires admin approval, Supplier requires admin approval, Customer requires no approval
                    IsApproved = model.Role == "Customer"
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                    
                    if (model.Role == "Customer")
                    {
                        // Customers are auto-approved; sign them in
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Customer");
                    }
                    
                    // Manufacturers and Suppliers require admin approval
                    TempData["SuccessMessage"] = "Registration submitted! Please wait for admin approval.";
                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Ensure available roles are set correctly
            model.AvailableRoles = new List<string> { "Manufacturer", "Supplier", "Customer" };
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            var model = new LoginViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    
                    if (user != null && user.IsApproved)
                    {
                        return RedirectToAction("Index", user.Role);
                    }
                    else if (user != null && !user.IsApproved)
                    {
                        await _signInManager.SignOutAsync();
                        TempData["ErrorMessage"] = "Your account is pending approval from the manufacturer.";
                        return View(model);
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new ProfileViewModel
            {
                Name = user.Name,
                CompanyName = user.CompanyName,
                DisplayName = user.DisplayName,
                Email = user.Email ?? string.Empty,
                Role = user.Role,
                IsApproved = user.IsApproved,
                CreatedAt = user.CreatedAt
            };

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new EditProfileViewModel
            {
                Name = user.Name,
                CompanyName = user.CompanyName,
                Email = user.Email ?? string.Empty
            };

            ViewBag.UserRole = user.Role;
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound();

                user.Name = user.Role == "Customer" ? model.Name : null;
                user.CompanyName = user.Role != "Customer" ? model.CompanyName : null;
                user.Email = model.Email;
                user.UserName = model.Email;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Profile updated successfully.";
                    return RedirectToAction("Profile");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
    }
}
