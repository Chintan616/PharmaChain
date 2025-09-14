using Microsoft.AspNetCore.Identity;
using PharmaChain.Models;

namespace PharmaChain.Services
{
    public class SeedDataService
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Create roles
            string[] roles = { "Manufacturer", "Supplier", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create default manufacturer
            var manufacturerEmail = "admin@pharmachain.com";
            var manufacturer = await userManager.FindByEmailAsync(manufacturerEmail);
            
            if (manufacturer == null)
            {
                manufacturer = new ApplicationUser
                {
                    UserName = manufacturerEmail,
                    Email = manufacturerEmail,
                    Name = "System Administrator",
                    Role = "Manufacturer",
                    IsApproved = true,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(manufacturer, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(manufacturer, "Manufacturer");
                }
            }
        }
    }
}
