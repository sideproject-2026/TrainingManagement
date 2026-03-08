using System.Net;

namespace BuildingBlock.Util.Commons.Results;

public sealed class Error
{
    public Error(
        string title,
        HttpStatusCode statusCode,
        string? detail = null,
        string? type = null,
        string? instance = null,
        IDictionary<string, string[]>? errors = null)
    {
        Title = string.IsNullOrWhiteSpace(title) ? "An error occurred." : title;
        StatusCode = statusCode;
        Status = (int)statusCode;
        Detail = detail;
        Type = string.IsNullOrWhiteSpace(type) ? "about:blank" : type;
        Instance = instance;
        Errors = errors ?? new Dictionary<string, string[]>();
    }

    public string Type { get; }

    public string Title { get; }

    public int Status { get; }

    public HttpStatusCode StatusCode { get; }

    public string? Detail { get; }

    public string? Instance { get; }

    public IDictionary<string, string[]> Errors { get; }

    public static Error FromProblemDetails(
        HttpStatusCode statusCode,
        string title,
        string? detail = null,
        string? instance = null,
        string? type = null,
        IDictionary<string, string[]>? errors = null)
        => new(title, statusCode, detail, type, instance, errors);

    public static Error Validation(
        IDictionary<string, string[]> errors,
        string? detail = null,
        string? instance = null)
        => new(
            "Validation error",
            HttpStatusCode.BadRequest,
            detail,
            "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            instance,
            errors);
}
