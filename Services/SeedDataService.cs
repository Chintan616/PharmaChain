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

            // Create default users
            var users = new[]
            {
                new { Email = "admin@pharmachain.com", Password = "Admin@123", Role = "Admin", CompanyName = "PharmaChain Systems", Name = (string?)null, IsApproved = true },
                new { Email = "customer@pharmachain.com", Password = "Customer@123", Role = "Customer", CompanyName = (string?)null, Name = "Customer", IsApproved = true },
                new { Email = "manufacturer@pharmachain.com", Password = "Manufacturer@123", Role = "Manufacturer", CompanyName = "Sun Pharma", Name = (string?)null, IsApproved = true },
                new { Email = "supplier@pharmachain.com", Password = "Supplier@123", Role = "Supplier", CompanyName = "Apollo Pharmacy", Name = (string?)null, IsApproved = true }
            };

            foreach (var userData in users)
            {
                var user = await userManager.FindByEmailAsync(userData.Email);
                
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = userData.Email,
                        Email = userData.Email,
                        Name = userData.Name,
                        CompanyName = userData.CompanyName,
                        Role = userData.Role,
                        IsApproved = userData.IsApproved,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, userData.Password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, userData.Role);
                    }
                }
                else
                {
                    // Update existing user with proper role and approval status
                    user.Role = userData.Role;
                    user.IsApproved = userData.IsApproved;
                    user.CompanyName = userData.CompanyName;
                    user.Name = userData.Name;
                    await userManager.UpdateAsync(user);
                    
                    // Ensure user is in the correct role
                    var currentRoles = await userManager.GetRolesAsync(user);
                    if (!currentRoles.Contains(userData.Role))
                    {
                        if (currentRoles.Any())
                        {
                            await userManager.RemoveFromRolesAsync(user, currentRoles);
                        }
                        await userManager.AddToRoleAsync(user, userData.Role);
                    }
                }
            }
        }
    }
}
