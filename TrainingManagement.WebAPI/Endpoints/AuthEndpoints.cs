using Carter;

using Microsoft.AspNetCore.Mvc;
using TrainingManagement.Auth.Contracts;
using TrainingManagement.WebAPI.Commons.Dtos;
using Microsoft.AspNetCore.Http;

namespace TrainingManagement.WebAPI.Endpoints;

public class AuthEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var appGroup = app.MapGroup("/api/auth")
            .WithTags("Authentication");

        appGroup.MapPost("/", async (
            [FromBody] AuthRequest request,
            [FromServices] IAuthService service,
            CancellationToken ct = default) =>
        {
            var result = await service.LoginAsync(request.UserName, request.Password, ct);

            if (result.Failed)
            {
                return Results.BadRequest(result.Errors);
            }

            return Results.Ok(new AuthResponse(
                result.Token!,
                result.Data?.UserName ?? string.Empty,
                result.ExpiresAt?.UtcDateTime ?? DateTime.MinValue));
        }).AllowAnonymous();
    }
}
