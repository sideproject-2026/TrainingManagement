using System.Threading;
using TrainingManagement.Auth.Commons.Results;
using TrainingManagement.Auth.Models;

namespace TrainingManagement.Auth.Contracts;

public interface IAuthService
{
    /// <summary>
    /// LoginAsync is a method that takes in a username and password, and returns 
    /// a LoginResult object that contains the result of the login attempt. 
    /// The LoginResult object contains information about whether the login was successful, any errors that occurred, 
    /// and if successful, the token and user data.
    /// 
    /// </summary>
    /// <param name="userName">The username of the user attempting to log in.</param>
    /// <param name="password">The password of the user attempting to log in.</param>
    /// <returns>A LoginResult object containing the result of the login attempt.</returns>
    Task<LoginResult<AppUser>> LoginAsync(string userName, string password, CancellationToken ct = default); 

    /// <summary>
    /// Asynchronously refreshes the access token using the provided refresh token.
    /// </summary>
    /// <param name="token">The expired or soon-to-expire access token to be refreshed.</param>
    /// <param name="refreshToken">The refresh token associated with the access token. Must be valid and not expired.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the refresh operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see
    /// cref="RefreshTokenResult{AppUser}"/> with the refreshed access token and user information if the operation
    /// succeeds.</returns>
    Task<RefreshTokenResult<AppUser>> RefreshTokenAsync(string token, string refreshToken, CancellationToken ct = default);

    /// <summary>
    /// Logs out the specified user asynchronously.
    /// </summary>
    /// <param name="userName">The user name of the account to log out. Cannot be null or empty.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the logout operation.</param>
    /// <returns>A task that represents the asynchronous logout operation. The task result contains a value indicating whether
    /// the logout was successful.</returns>
    Task<Result> LogoutAsync(string userName, CancellationToken ct = default);
    
    /// <summary>
    /// Validates the specified token asynchronously.
    /// </summary>
    /// <param name="token">The token to be validated.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the validation operation.</param>
    /// <returns>A task that represents the asynchronous token validation operation. The task result contains a value indicating whether
    /// the token is valid.</returns>
    Task<Result> TokenValidation(string token, CancellationToken ct = default);
}
