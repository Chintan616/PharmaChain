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
            string[] roles = { "Admin", "Manufacturer", "Supplier", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create default admin
            var adminEmail = "admin@pharmachain.com";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Name = null,
                    CompanyName = "PharmaChain Systems",
                    Role = "Admin",
                    IsApproved = true,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}
