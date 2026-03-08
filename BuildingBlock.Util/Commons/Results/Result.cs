using System.Net;

namespace BuildingBlock.Util.Commons.Results;

public class Result
{
    protected Result(bool succeeded, HttpStatusCode statusCode, IDictionary<string, string[]> errors)
        : this(succeeded, statusCode, new[] { new Error("Error", statusCode, errors: errors) })
    {
    }

    protected Result(bool succeeded, HttpStatusCode statusCode, IReadOnlyCollection<Error> errors)
    {
        Succeeded = succeeded;
        StatusCode = statusCode;
        Errors = errors;
    }

    public bool Succeeded { get; }

    public bool Failed => !Succeeded;

    public HttpStatusCode StatusCode { get; }

    public IReadOnlyCollection<Error> Errors { get; }

    public static Result Success(HttpStatusCode statusCode = HttpStatusCode.OK)
        => new(true, statusCode, Array.Empty<Error>());

    public static Result Fail(HttpStatusCode statusCode = HttpStatusCode.BadRequest, params string[] errors)
    {
        var normalizedErrors = NormalizeErrors(errors);
        return new Result(false, statusCode, CreateProblemErrors(statusCode, normalizedErrors));
    }

    public static Result Fail(HttpStatusCode statusCode, IDictionary<string, string[]> errors)
    {
        var normalizedErrors = errors is { Count: > 0 } ? errors : new Dictionary<string, string[]> { { "message", new[] { "Operation failed." } } };
        return new Result(false, statusCode, new[] { new Error("Operation failed.", statusCode, errors: normalizedErrors) });
    }

    public static Result Fail(HttpStatusCode statusCode, params Error[] errors)
    {
        var normalizedErrors = errors is { Length: > 0 }
            ? errors
            : new[] { new Error("Operation failed.", statusCode) };

        return new Result(false, statusCode, normalizedErrors);
    }

    public Result<T> WithValue<T>(T value, HttpStatusCode? statusCode = null)
    {
        return new Result<T>(value, statusCode ?? StatusCode, Errors, Succeeded);
    }

    protected static string[] NormalizeErrors(string[] errors)
        => errors is { Length: > 0 }
            ? errors
            : new[] { "Operation failed." };

    protected static Error[] CreateProblemErrors(HttpStatusCode statusCode, string[] messages)
        => new[]
        {
            new Error(
                "Operation failed.",
                statusCode,
                detail: string.Join("; ", messages),
                errors: new Dictionary<string, string[]> { { "message", messages } })
        };
}

public sealed class Result<T> : Result
{
    internal Result(T value, HttpStatusCode statusCode, IReadOnlyCollection<Error> errors, bool succeeded)
        : base(succeeded, statusCode, errors)
    {
        Value = value;
    }

    public T Value { get; }

    public static Result<T> Success(T value, HttpStatusCode statusCode = HttpStatusCode.OK)
        => new(value, statusCode, Array.Empty<Error>(), true);

    public static new Result<T> Fail(HttpStatusCode statusCode = HttpStatusCode.BadRequest, params string[] errors)
    {
        var normalizedErrors = NormalizeErrors(errors);
        return new Result<T>(default!, statusCode, CreateProblemErrors(statusCode, normalizedErrors), false);
    }

    public static Result<T> Fail(HttpStatusCode statusCode, params Error[] errors)
    {
        var normalizedErrors = errors is { Length: > 0 }
            ? errors
            : new[] { new Error("Operation failed.", statusCode) };

        return new Result<T>(default!, statusCode, normalizedErrors, false);
    }

    public static Result<T> Fail(HttpStatusCode statusCode, IDictionary<string, string[]> errors)
    {
        var normalizedErrors = errors is { Count: > 0 } ? errors : new Dictionary<string, string[]> { { "message", new[] { "Operation failed." } } };
        return new Result<T>(default!, statusCode, new[] { new Error("Operation failed.", statusCode, errors: normalizedErrors) }, false);
    }
}
