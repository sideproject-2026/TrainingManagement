using BuildingBlock.Util.Commons.Results;
using Carter;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TrainingManagement.Auth.Contracts;
using TrainingManagement.WebAPI.Commons.Dtos;
using TrainingManagement.WebAPI.Commons.Errors;

namespace TrainingManagement.WebAPI.Endpoints;

public class AuthEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var version = app.NewVersionedApi();
        var appGroup = version.MapGroup("/api/v{version:apiVersion}/auth")
            .HasApiVersion(1.0)
            .WithTags("Authentication");

        appGroup.MapPost("/", HandleLoginAsync)
            .MapToApiVersion(1.0)
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
       
        if (result.Failed) {
            //return proper error response with details
            var error = new Error("Invalid Credential", System.Net.HttpStatusCode.Unauthorized);
            return ResultHandler
                    .Handle(Result<AuthResponse>.Fail(HttpStatusCode.Unauthorized, error));
        }

        var response = new AuthResponse(
                result.Token!,
                result.Data?.UserName ?? string.Empty,
                result.ExpiresAt?.UtcDateTime ?? DateTime.MinValue,
                result.RefreshToken ?? string.Empty,
                result.RefreshExpiresAt?.UtcDateTime ?? DateTime.MinValue);

        return ResultHandler
                .Handle(Result<AuthResponse>.Success(response));
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
