using System;
using System.Collections.Generic;
using System.Net;

namespace TrainingManagement.Auth.Commons.Results;

public class Result
{
    protected Result(bool succeeded, HttpStatusCode statusCode, IReadOnlyCollection<string> errors)
    {
        Succeeded = succeeded;
        StatusCode = statusCode;
        Errors = errors;
    }

    public bool Succeeded { get; }

    public bool Failed => !Succeeded;

    public HttpStatusCode StatusCode { get; }

    public IReadOnlyCollection<string> Errors { get; }

    public static Result Success(HttpStatusCode statusCode = HttpStatusCode.OK)
        => new(true, statusCode, Array.Empty<string>());

    public static Result Fail(HttpStatusCode statusCode = HttpStatusCode.BadRequest, params string[] errors)
    {
        var normalizedErrors = errors is { Length: > 0 }
            ? errors
            : new[] { "Operation failed." };

        return new Result(false, statusCode, normalizedErrors);
    }

    public Result<T> WithValue<T>(T value, HttpStatusCode? statusCode = null)
    {
        return new Result<T>(value, statusCode ?? StatusCode, Errors, Succeeded);
    }
}

public sealed class Result<T> : Result
{
    internal Result(T value, HttpStatusCode statusCode, IReadOnlyCollection<string> errors, bool succeeded)
        : base(succeeded, statusCode, errors)
    {
        Value = value;
    }

    public T Value { get; }

    public static Result<T> Success(T value, HttpStatusCode statusCode = HttpStatusCode.OK)
        => new(value, statusCode, Array.Empty<string>(), true);

    public static new Result<T> Fail(HttpStatusCode statusCode = HttpStatusCode.BadRequest, params string[] errors)
    {
        var normalizedErrors = errors is { Length: > 0 }
            ? errors
            : new[] { "Operation failed." };

        return new Result<T>(default!, statusCode, normalizedErrors, false);
    }
}
