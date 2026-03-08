using Microsoft.AspNetCore.Identity;
using TrainingManagement.Auth.Commons.Results;
using TrainingManagement.Auth.Commons.Security;
using TrainingManagement.Auth.Contracts;
using TrainingManagement.Auth.Models;

namespace TrainingManagement.Auth.Services;

internal class UserService(UserManager<AppUser> userManager) : IUserService
{
    public async Task<Result<AppUser>> SignUpAsync(AppUser user, IEnumerable<string> roles, string password)
    {
        var result = await userManager.CreateAsync(user, password);
        if(!result.Succeeded)
        {
            return Result<AppUser>.Fail(
                System.Net.HttpStatusCode.BadRequest, 
                result.Errors.Select(e => e.Description).ToArray());
        }

        //add training claim
        await userManager.AddClaimAsync(user, new System.Security.Claims.Claim(TrainingCenterClaimType.TrainingCenter, user.TainingCenterId.ToString()));
        await userManager.AddClaimAsync(user, new System.Security.Claims.Claim(TrainingCenterClaimType.UserType, user.UserType.ToString()));

        //add roles
        if(roles.Any())
        {
            var roleResult = await userManager.AddToRolesAsync(user, roles);
            if(!roleResult.Succeeded)
            {
                return Result<AppUser>.Fail(
                    System.Net.HttpStatusCode.BadRequest, 
                    roleResult.Errors.Select(e => e.Description).ToArray());
            }
        }

        return Result<AppUser>.Success(user);
    }
}
