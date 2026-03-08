using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using TrainingManagement.Auth.Commons.Results;
using TrainingManagement.Auth.Commons.Security;
using TrainingManagement.Auth.Contracts;
using TrainingManagement.Auth.Models;
using TrainingManagement.Auth.Persistence;

namespace TrainingManagement.Auth.Services
{
    internal sealed class AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration, AuthDbContext dbContext) : IAuthService
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly SignInManager<AppUser> _signInManager = signInManager;
        private readonly IConfiguration _configuration = configuration;
        private readonly AuthDbContext _dbContext = dbContext;

        public async Task<LoginResult<AppUser>> LoginAsync(string userName, string password, CancellationToken ct = default)
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

           //add user claim 
           var claims = await _userManager.GetClaimsAsync(user);
          

            var roles = await _userManager.GetRolesAsync(user);

            var jwtSection = _configuration.GetSection("Jwt");
            var signingKey = jwtSection["Key"] ?? string.Empty;
            var issuer = jwtSection["Issuer"] ?? string.Empty;
            var audience = jwtSection["Audience"] ?? string.Empty;
            var expiresMinutes = int.TryParse(jwtSection["ExpiresMinutes"], out var minutes) && minutes > 0
                ? minutes
                : 60;
            var refreshExpiresMinutes = int.TryParse(jwtSection["RefreshExpiresMinutes"], out var refreshMinutes) && refreshMinutes > 0
                ? refreshMinutes
                : 60 * 24 * 7; // default 7 days

            if (string.IsNullOrWhiteSpace(signingKey) || string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
            {
                return LoginResult<AppUser>.Fail("Authentication is not configured correctly.");
            }

            var expiresAt = DateTimeOffset.UtcNow.AddMinutes(expiresMinutes);
            var refreshExpiresAt = DateTimeOffset.UtcNow.AddMinutes(refreshExpiresMinutes);

            var token = JWTTokenGenerator.GenerateToken(
                user,
                roles,
                signingKey,
                issuer,
                audience,
                TimeSpan.FromMinutes(expiresMinutes),
                claims);

            var refreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                UserId = user.Id,
                ExpiresAt = refreshExpiresAt,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await RemoveExpiredTokensAsync(user.Id, ct);
            _dbContext.RefreshTokens.Add(refreshToken);
            await _dbContext.SaveChangesAsync(ct);

            return LoginResult<AppUser>.Success(token, user, expiresAt, refreshToken.Token, refreshExpiresAt);
        }

        public async Task<RefreshTokenResult<AppUser>> RefreshTokenAsync(string token, string refreshToken, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(refreshToken))
            {
                return RefreshTokenResult<AppUser>.Fail("Token and refresh token are required.");
            }

            var storedToken = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken, ct);

            if (storedToken is null)
            {
                return RefreshTokenResult<AppUser>.Fail("Invalid refresh token.");
            }

            if (storedToken.RevokedAt.HasValue)
            {
                return RefreshTokenResult<AppUser>.Fail("Refresh token has been revoked.");
            }

            if (storedToken.ExpiresAt <= DateTimeOffset.UtcNow)
            {
                return RefreshTokenResult<AppUser>.Fail("Refresh token has expired.");
            }

            var user = await _userManager.FindByIdAsync(storedToken.UserId);
            if (user is null)
            {
                return RefreshTokenResult<AppUser>.Fail("User not found.");
            }

            //add user claim 
            var claims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var jwtSection = _configuration.GetSection("Jwt");
            var signingKey = jwtSection["Key"] ?? string.Empty;
            var issuer = jwtSection["Issuer"] ?? string.Empty;
            var audience = jwtSection["Audience"] ?? string.Empty;
            var expiresMinutes = int.TryParse(jwtSection["ExpiresMinutes"], out var minutes) && minutes > 0
                ? minutes
                : 60;
            var refreshExpiresMinutes = int.TryParse(jwtSection["RefreshExpiresMinutes"], out var refreshMinutes) && refreshMinutes > 0
                ? refreshMinutes
                : 60 * 24 * 7;

            if (string.IsNullOrWhiteSpace(signingKey) || string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
            {
                return RefreshTokenResult<AppUser>.Fail("Authentication is not configured correctly.");
            }

            var expiresAt = DateTimeOffset.UtcNow.AddMinutes(expiresMinutes);
            var refreshExpiresAt = DateTimeOffset.UtcNow.AddMinutes(refreshExpiresMinutes);

            var newRefreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                UserId = user.Id,
                ExpiresAt = refreshExpiresAt,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _dbContext.RefreshTokens.Remove(storedToken);

            await RemoveExpiredTokensAsync(user.Id, ct);
            _dbContext.RefreshTokens.Add(newRefreshToken);
            await _dbContext.SaveChangesAsync(ct);

            var newAccessToken = JWTTokenGenerator.GenerateToken(
                user,
                roles,
                signingKey,
                issuer,
                audience,
                TimeSpan.FromMinutes(expiresMinutes),
                claims);

            return RefreshTokenResult<AppUser>.Success(newAccessToken, newRefreshToken.Token, expiresAt, refreshExpiresAt, user);
        }

        public async Task<Result> LogoutAsync(string userName, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return Result.Fail(System.Net.HttpStatusCode.BadRequest, "Username is required.");
            }

            var user = await _userManager.FindByNameAsync(userName);
            if (user is null)
            {
                return Result.Fail(System.Net.HttpStatusCode.NotFound, "User not found.");
            }

            await _signInManager.SignOutAsync();

            var userTokens = await _dbContext.RefreshTokens
                .Where(rt => rt.UserId == user.Id)
                .ToListAsync(ct);

            if (userTokens.Count > 0)
            {
                _dbContext.RefreshTokens.RemoveRange(userTokens);
                await _dbContext.SaveChangesAsync(ct);
            }

            return Result.Success(System.Net.HttpStatusCode.OK);
        }

        public Task<Result> TokenValidation(string token, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Task.FromResult(Result.Fail(System.Net.HttpStatusCode.BadRequest, "Token is required."));
            }

            try
            {
                var parameters = BuildTokenValidationParameters();
                var handler = new JwtSecurityTokenHandler();
                handler.ValidateToken(token, parameters, out _);
                return Task.FromResult(Result.Success(System.Net.HttpStatusCode.OK));
            }
            catch (SecurityTokenExpiredException)
            {
                return Task.FromResult(Result.Fail(System.Net.HttpStatusCode.Unauthorized, "Token has expired."));
            }
            catch (Exception)
            {
                return Task.FromResult(Result.Fail(System.Net.HttpStatusCode.Unauthorized, "Token is invalid."));
            }
        }

        private static string GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes);
        }

        private async Task RemoveExpiredTokensAsync(string userId, CancellationToken ct)
        {
            var expiredTokens = await _dbContext.RefreshTokens
                .Where(rt => rt.UserId == userId && (rt.ExpiresAt <= DateTimeOffset.UtcNow || rt.RevokedAt.HasValue))
                .ToListAsync(ct);

            if (expiredTokens.Count > 0)
            {
                _dbContext.RefreshTokens.RemoveRange(expiredTokens);
            }
        }

        private TokenValidationParameters BuildTokenValidationParameters()
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var signingKey = jwtSection["Key"] ?? string.Empty;
            var issuer = jwtSection["Issuer"] ?? string.Empty;
            var audience = jwtSection["Audience"] ?? string.Empty;

            var keyBytes = System.Text.Encoding.UTF8.GetBytes(signingKey);

            return new TokenValidationParameters
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
        }

       
       
    }
}
