using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TrainingManagement.Auth.Models;

namespace TrainingManagement.Auth.Persistence.Seeds;

public static class DataSeed
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        var roles = new[] { "Admin", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        const string adminEmail = "admin@training.local";
        const string adminUserName = "admin";
        const string adminPassword = "Admin@123";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser is null)
        {
            adminUser = new AppUser
            {
                UserName = adminUserName,
                Email = adminEmail,
                EmailConfirmed = true,
                UserCode = "ADM001",
                FullName = "System Administrator",
                TainingCenterId = Guid.NewGuid()
            };

            var createResult = await userManager.CreateAsync(adminUser, adminPassword);

            if (createResult.Succeeded)
            {
                await userManager.AddToRolesAsync(adminUser, roles);
            }
        }
        else
        {
            var userRoles = await userManager.GetRolesAsync(adminUser);
            var missingRoles = roles.Except(userRoles).ToList();
            if (missingRoles.Count > 0)
            {
                await userManager.AddToRolesAsync(adminUser, missingRoles);
            }
        }
    }
}
