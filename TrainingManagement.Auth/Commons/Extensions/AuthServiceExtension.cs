using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TrainingManagement.Auth.Contracts;
using TrainingManagement.Auth.Models;
using TrainingManagement.Auth.Persistence;
using TrainingManagement.Auth.Services;

namespace TrainingManagement.Auth.Commons.Extensions;

public static class AuthServiceExtension
{
    public static IServiceCollection AddTMAuthService(this IServiceCollection services,IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<AuthDbContext>(options =>
            options.UseSqlServer(connectionString));

        services
            .AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();

        var jwtSection = configuration.GetSection("Jwt");
        var issuer = jwtSection["Issuer"]
            ?? throw new InvalidOperationException("JWT issuer is not configured (Jwt:Issuer).");
        var audience = jwtSection["Audience"]
            ?? throw new InvalidOperationException("JWT audience is not configured (Jwt:Audience).");
        var signingKey = jwtSection["Key"]
            ?? throw new InvalidOperationException("JWT signing key is not configured (Jwt:Key).");

        var keyBytes = Encoding.UTF8.GetBytes(signingKey);

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }  
}
