using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PharmaChain.Data;
using PharmaChain.Models;

namespace PharmaChain.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly ApplicationDbContext _context;

        protected BaseController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        protected void SetNoCacheHeaders()
        {
            Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            Response.Headers.Add("Pragma", "no-cache");
            Response.Headers.Add("Expires", "0");
        }
        //get current user
        protected async Task<ApplicationUser?> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }
        //check role
        protected async Task<bool> IsInRoleAsync(string role)
        {
            var user = await GetCurrentUserAsync();
            return user != null && await _userManager.IsInRoleAsync(user, role);
        }
        //check if user is approved
        protected async Task<bool> IsApprovedAsync()
        {
            var user = await GetCurrentUserAsync();
            return user?.IsApproved ?? false;
        }
    }
}
