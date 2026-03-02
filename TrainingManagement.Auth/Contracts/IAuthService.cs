using System.Threading;
using TrainingManagement.Auth.Commons.Results;
using TrainingManagement.Auth.Models;

namespace TrainingManagement.Auth.Contracts;

public interface IAuthService
{
    Task<LoginResult<AppUser>> LoginAsync(string userName, string password, CancellationToken ct = default); 
}
