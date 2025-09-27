using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PharmaChain.Models;
using System;
using System.Threading.Tasks;

namespace PharmaChain.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = { "Admin", "Manufacturer", "Supplier", "Customer" };

            // Ensure roles exist
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create default admin user
            var adminEmail = "admin@pharmachain.com";
            var adminPassword = "Admin@123"; // ✅ change to a stronger password for production

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    Name = "System Admin",
                    CompanyName = "PharmaChain HQ",
                    Role = "Admin"
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
