namespace TrainingManagement.WebAPI.Commons.Dtos
{
    public record AuthRequest(string UserName, string Password);

    public record AuthResponse(string Token, string UserName, DateTime Expiration, string RefreshToken, DateTime RefreshExpiration);

    public record RefreshTokenRequest(string Token, string RefreshToken);
}
