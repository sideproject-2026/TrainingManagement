using TrainingManagement.Auth.Commons.Results;
using TrainingManagement.Auth.Models;

namespace TrainingManagement.Auth.Contracts;

public interface IUserService
{
    public Task<Result<AppUser>> SignUpAsync(AppUser user,string password);
}
