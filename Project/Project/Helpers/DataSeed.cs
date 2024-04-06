using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Project.Data;
using Project.Entities;
using System;
using System.Threading.Tasks;

namespace Project.Helpers
{
    public static class DataSeed
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                string[] roles = new[] { "Admin", "User" };

                foreach (string role in roles)
                {
                    var exists = await roleManager.RoleExistsAsync(role);
                    if (exists) continue;
                    await roleManager.CreateAsync(new IdentityRole(role));
                }

                var user = new User
                {
                    Email = "admin@gmail.com",
                    UserName = "Admin0012",
                    Name = "Admin User" // Provide a name for the user
                };

                var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var existingUser = await _userManager.FindByEmailAsync(user.Email);
                if (existingUser != null) return;

                var result = await _userManager.CreateAsync(user, "murad2006");
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create user: {string.Join(", ", result.Errors)}");
                }

                await _userManager.AddToRoleAsync(user, roles[0]);

                return;
            }
        }
    }
}
