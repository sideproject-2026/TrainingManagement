using System;
using System.Collections.Generic;

namespace TrainingManagement.Auth.Commons.Results;

public sealed class LoginResult<T>
{
    private LoginResult(bool succeeded, string? token, T? data, DateTimeOffset? expiresAt, IReadOnlyCollection<string> errors)
    {
        Succeeded = succeeded;
        Token = token;
        Data = data;
        ExpiresAt = expiresAt;
        Errors = errors;
    }

    public bool Succeeded { get; }

    public bool Failed => !Succeeded;

    public string? Token { get; }

    public T? Data { get; }

    public DateTimeOffset? ExpiresAt { get; }

    public IReadOnlyCollection<string> Errors { get; }

    public static LoginResult<T> Success(string token, T data, DateTimeOffset expiresAt) => new(true, token, data, expiresAt, []);

    public static LoginResult<T> Fail(params string[] errors)
    {
        var normalizedErrors = errors is { Length: > 0 }
            ? errors
            : ["Login failed."];

        return new LoginResult<T>(false, null, default, null, normalizedErrors);
    }
}
