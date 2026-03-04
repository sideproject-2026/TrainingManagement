using Microsoft.EntityFrameworkCore;
using TrainingManagement.Auth.Persistence;

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
            app.Logger.LogError(ex, "An error occurred while migrating the database");
            throw;
        }
    }
}
