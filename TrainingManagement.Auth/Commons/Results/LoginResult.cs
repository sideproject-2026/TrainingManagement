using System;
using System.Collections.Generic;

namespace TrainingManagement.Auth.Commons.Results;

public sealed class LoginResult<T>
{
    private LoginResult(bool succeeded, string? token, T? data, DateTimeOffset? expiresAt, string? refreshToken, DateTimeOffset? refreshExpiresAt, IReadOnlyCollection<string> errors)
    {
        Succeeded = succeeded;
        Token = token;
        Data = data;
        ExpiresAt = expiresAt;
        RefreshToken = refreshToken;
        RefreshExpiresAt = refreshExpiresAt;
        Errors = errors;
    }

    public bool Succeeded { get; }

    public bool Failed => !Succeeded;

    public string? Token { get; }

    public T? Data { get; }

    public DateTimeOffset? ExpiresAt { get; }

    public string? RefreshToken { get; }

    public DateTimeOffset? RefreshExpiresAt { get; }

    public IReadOnlyCollection<string> Errors { get; }

    public static LoginResult<T> Success(string token, T data, DateTimeOffset expiresAt, string refreshToken, DateTimeOffset refreshExpiresAt)
        => new(true, token, data, expiresAt, refreshToken, refreshExpiresAt, []);

    public static LoginResult<T> Fail(params string[] errors)
    {
        var normalizedErrors = errors is { Length: > 0 }
            ? errors
            : ["Login failed."];

        return new LoginResult<T>(false, null, default, null, null, null, normalizedErrors);
    }
}
