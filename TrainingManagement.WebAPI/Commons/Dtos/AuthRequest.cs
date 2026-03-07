using System.ComponentModel.DataAnnotations;

namespace TrainingManagement.WebAPI.Commons.Dtos
{
    public record AuthRequest(
        [property: Required(ErrorMessage = "Username is required.")]
        string UserName,
        [property: Required(ErrorMessage = "Password is required.")]
        string Password);

    public record AuthResponse(string Token, string UserName, DateTime Expiration, string RefreshToken, DateTime RefreshExpiration);

    public record RefreshTokenRequest(
        [property: Required(ErrorMessage = "Token is required.")]
        string Token,
        [property: Required(ErrorMessage = "Refresh token is required.")]
        string RefreshToken);
}
