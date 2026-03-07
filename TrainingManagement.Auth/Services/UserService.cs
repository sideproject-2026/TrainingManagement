using Microsoft.AspNetCore.Identity;
using TrainingManagement.Auth.Commons.Results;
using TrainingManagement.Auth.Contracts;
using TrainingManagement.Auth.Models;

namespace TrainingManagement.Auth.Services;

internal class UserService(UserManager<AppUser> userManager) : IUserService
{
    public async Task<Result<AppUser>> SignUp(AppUser user,string password)
    {
        var result = await userManager.CreateAsync(user, password);
        if(!result.Succeeded)
        {
            return Result<AppUser>.Fail(
                System.Net.HttpStatusCode.BadRequest, 
                result.Errors.Select(e => e.Description).ToArray());
        }
        
        return Result<AppUser>.Success(user);
    }
}
