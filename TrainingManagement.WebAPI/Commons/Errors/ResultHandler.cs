using BuildingBlock.Util.Commons.Results;

namespace TrainingManagement.WebAPI.Commons.Errors;

public static class ResultHandler
{

    public static IResult Handle<T>(Result<T> result)
    {
        if(result.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
            return Results.Unauthorized();
        }

        if(result.StatusCode == System.Net.HttpStatusCode.NotFound) {
            return Results.NotFound(result.Errors);
        }

        if(result.StatusCode == System.Net.HttpStatusCode.Conflict) {
            return Results.Conflict(result.Errors);
        }

        if (result.Failed)
        {
            // Here you can customize the error response based on the type of errors
            // For example, you could check for specific error types and return different status codes
           
            return Results.BadRequest(result.Errors);
        }

        if(result.StatusCode == System.Net.HttpStatusCode.Created) {
            if(result.Value == null)  {
                return Results.Created();
            }

            return Results.Created(string.Empty, result.Value);
        }

        return Results.Ok(result.Value);
    }
}
