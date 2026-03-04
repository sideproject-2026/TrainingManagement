using System;
using System.Collections.Generic;

namespace TrainingManagement.Auth.Commons.Results;

public sealed class RefreshTokenResult<T>
{
    private RefreshTokenResult(bool succeeded, string? token, string? refreshToken, DateTimeOffset? expiresAt, DateTimeOffset? refreshExpiresAt, T? data, IReadOnlyCollection<string> errors)
    {
        Succeeded = succeeded;
        Token = token;
        RefreshToken = refreshToken;
        ExpiresAt = expiresAt;
        RefreshExpiresAt = refreshExpiresAt;
        Data = data;
        Errors = errors;
    }

    public bool Succeeded { get; }

    public bool Failed => !Succeeded;

    public string? Token { get; }

    public string? RefreshToken { get; }

    public DateTimeOffset? ExpiresAt { get; }

    public DateTimeOffset? RefreshExpiresAt { get; }

    public T? Data { get; }

    public IReadOnlyCollection<string> Errors { get; }

    public static RefreshTokenResult<T> Success(string token, string refreshToken, DateTimeOffset expiresAt, DateTimeOffset refreshExpiresAt, T data)
        => new(true, token, refreshToken, expiresAt, refreshExpiresAt, data, []);

    public static RefreshTokenResult<T> Fail(params string[] errors)
    {
        var normalizedErrors = errors is { Length: > 0 }
            ? errors
            : ["Refresh token failed."];

        return new RefreshTokenResult<T>(false, null, null, null, null, default, normalizedErrors);
    }
}
