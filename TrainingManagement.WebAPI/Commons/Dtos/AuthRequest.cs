namespace TrainingManagement.WebAPI.Commons.Dtos
{
    public record AuthRequest(string UserName, string Password);

    public record AuthResponse(string Token, string UserName,DateTime Expiration);
}
