using BuildingBlock.Util.Commons.Parameters;
using Carter;
using Microsoft.AspNetCore.Mvc;
using TrainingManagement.Auth.Contracts;
using TrainingManagement.Auth.Models;
using TrainingManagement.Center.Contracts;
using TrainingManagement.Center.Models;
using TrainingManagement.WebAPI.Commons.Dtos;

namespace TrainingManagement.WebAPI.Endpoints;
public class TrainingCenterEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var appGroup = app.MapGroup("/api/centers")
       .WithTags("Training Center");


        appGroup.MapPost("/create", HandleCreateAsync);
        appGroup.MapGet("/", HandleGetAllAsync);
    }

    public async Task<IResult> HandleGetAllAsync(
        [FromServices] ICenterService service,
        CancellationToken ct = default)
    {
        var queryResult = await service.GetAllAsync(FilterParam.NoPaginateDefault, ct);
        return Results.Ok(new { Data = queryResult.Data, PageCount = queryResult.PageCount });
    }


    public async Task<IResult> HandleCreateAsync(
        [FromBody] TrainingCenterRequest request,
        [FromServices] ICenterService service,
        [FromServices] IUserService userService,
        CancellationToken ct = default)
    {
        var trainingCenter = TrainingCenter
                                .CreateNew(
                                    request.Name,
                                    request.Code, 
                                    request.Email, 
                                    request.Description, 
                                    request.Address, 
                                    request.Municipality, 
                                    request.ContactNo);
        
        var result = await service.CreateAsync(trainingCenter, ct);
        if (result.Failed) {
            return Results.BadRequest(result.Errors);
        }

        //create default admin user
        var trainingCenterId = result.Value;
        var defaultUser = $"Admin@{trainingCenter.Code}.com";
        var defaultPassword = $"Admin{trainingCenter.Code}@123";
        var userCode = $"TC{trainingCenter.Code}-ADMIN";
        
        var newUser = AppUser.Create(defaultUser, trainingCenter.Email, userCode, $"Admin-{trainingCenter.Code}", trainingCenterId);

        var userResult = await userService.SignUpAsync(newUser, defaultPassword);
        if(userResult.Failed)
        {
            //rollback training center creation
            await service.DeleteAsync(trainingCenterId, ct);
            return Results.BadRequest(userResult.Errors);
        }

        return Results.Ok(result);
    }
}