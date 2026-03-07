using Carter;
using Microsoft.AspNetCore.Mvc;
using TrainingManagement.Auth.Contracts;
using TrainingManagement.WebAPI.Commons.Dtos;

namespace TrainingManagement.WebAPI.Endpoints;

public class AuthEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var appGroup = app.MapGroup("/api/auth")
            .WithTags("Authentication");

        appGroup.MapPost("/", HandleLoginAsync)
            .AllowAnonymous();
        appGroup.MapPost("/refresh", HandleRefreshAsync)
            .AllowAnonymous(); // Ensure this endpoint is also anonymous
    }

    private static async Task<IResult> HandleLoginAsync(
        [FromBody] AuthRequest request,
        [FromServices] IAuthService service,
        CancellationToken ct = default)
    {
       
        

        var result = await service.LoginAsync(request.UserName, request.Password, ct);

        if (result.Failed)
        {
            return Results.BadRequest(result.Errors);
        }

        return Results.Ok(new AuthResponse(
            result.Token!,
            result.Data?.UserName ?? string.Empty,
            result.ExpiresAt?.UtcDateTime ?? DateTime.MinValue,
            result.RefreshToken ?? string.Empty,
            result.RefreshExpiresAt?.UtcDateTime ?? DateTime.MinValue));
    }

    private static async Task<IResult> HandleRefreshAsync(
        [FromBody] RefreshTokenRequest request,
        [FromServices] IAuthService service,
        CancellationToken ct = default)
    {   

        var result = await service.RefreshTokenAsync(request.Token, request.RefreshToken, ct);

        if (result.Failed)
        {
            return Results.BadRequest(result.Errors);
        }

        return Results.Ok(new AuthResponse(
            result.Token!,
            result.Data?.UserName ?? string.Empty,
            result.ExpiresAt?.UtcDateTime ?? DateTime.MinValue,
            result.RefreshToken ?? string.Empty,
            result.RefreshExpiresAt?.UtcDateTime ?? DateTime.MinValue));
    }

   
}
