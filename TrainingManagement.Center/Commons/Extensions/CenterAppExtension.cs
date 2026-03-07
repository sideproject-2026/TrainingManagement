using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrainingManagement.Center.Contracts;
using TrainingManagement.Center.Persistence;
using TrainingManagement.Center.Services;

namespace TrainingManagement.Center.Commons.Extensions;

public static class CenterAppExtension
{
    public static IServiceCollection AddCenterService(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddDbContext<CenterDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        services.AddScoped<ICenterService, CenterService>();
        return services;
    }
}
