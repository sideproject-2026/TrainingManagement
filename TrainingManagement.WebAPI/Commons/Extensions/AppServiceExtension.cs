using Microsoft.EntityFrameworkCore;
using TrainingManagement.Auth.Persistence;
using TrainingManagement.Auth.Persistence.Seeds;

namespace TrainingManagement.WebAPI.Commons.Extensions;


public static class AppServiceExtension
{

    public static async Task UseAuthMigration(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<AuthDbContext>();
            if(context.Database.GetPendingMigrations().Any())
            {
                await context.Database.MigrateAsync();
            }

            //await DataSeed.SeedAsync(services);
        }
        catch (Exception ex)
        {
            // Log the error (you can use your logging framework here)
            Console.WriteLine($"An error occurred while migrating the database: {ex.Message}");
            throw; // Re-throw the exception after logging
        }
    }
}
