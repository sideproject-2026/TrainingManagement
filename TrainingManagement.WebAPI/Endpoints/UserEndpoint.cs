using Carter;
using Microsoft.AspNetCore.Mvc;
using TrainingManagement.Auth.Contracts;
using TrainingManagement.Auth.Models;
using TrainingManagement.WebAPI.Commons.Dtos;

namespace TrainingManagement.WebAPI.Endpoints;

public class UserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var appGroup = app.MapGroup("/api/users")
          .WithTags("User");

        appGroup.MapPost("/create", HandleSignUp);
    }

    public static async Task<IResult> HandleSignUp(
        [FromBody] UserRequest request,
        [FromServices] IUserService service,
        CancellationToken ct = default)
    {
        var appUser = AppUser.Create(request.UserName,request.Email, request.UserCode, request.FullName, request.TrainingCenterId);
        var result = await service.SignUpAsync(appUser, request.Password);
        
        if (result.Failed)
        {
            return Results.BadRequest(result.Errors);
        }
        return Results.Ok();
    }


}
