using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using TrainingManagement.Auth.Commons.Results;
using TrainingManagement.Auth.Commons.Security;
using TrainingManagement.Auth.Contracts;
using TrainingManagement.Auth.Models;

namespace TrainingManagement.Auth.Services
{
    internal sealed class AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration) : IAuthService
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly SignInManager<AppUser> _signInManager = signInManager;
        private readonly IConfiguration _configuration = configuration;

        public async Task<LoginResult<AppUser>> LoginAsync(string userName, string password,CancellationToken ct = default)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user is null)
            {
                return LoginResult<AppUser>.Fail("Invalid credentials.");
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!signInResult.Succeeded)
            {
                return LoginResult<AppUser>.Fail("Invalid credentials.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var jwtSection = _configuration.GetSection("Jwt");
            var signingKey = jwtSection["Key"] ?? string.Empty;
            var issuer = jwtSection["Issuer"] ?? string.Empty;
            var audience = jwtSection["Audience"] ?? string.Empty;
            var expiresMinutes = int.TryParse(jwtSection["ExpiresMinutes"], out var minutes) && minutes > 0
                ? minutes
                : 60;

            if (string.IsNullOrWhiteSpace(signingKey) || string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
            {
                return LoginResult<AppUser>.Fail("Authentication is not configured correctly.");
            }

            var expiresAt = DateTimeOffset.UtcNow.AddMinutes(expiresMinutes);

            var token = JWTTokenGenerator.GenerateToken(
                user,
                roles,
                signingKey,
                issuer,
                audience,
                TimeSpan.FromMinutes(expiresMinutes));

            return LoginResult<AppUser>.Success(token, user, expiresAt);
        }
    }
}
