using System.Net;

namespace CreditFlow.Web.Core.Http;

public record ApiResult(
    bool IsSuccess,
    string? Message = null,
    HttpStatusCode? StatusCode = null,
    string? Code = null)
{
    public static ApiResult Success() => new(true);

    public static ApiResult Failure(
        string message,
        HttpStatusCode? statusCode = null,
        string? code = null) => new(false, message, statusCode, code);
}

public sealed record ApiResult<T>(
    bool IsSuccess,
    T? Data = default,
    string? Message = null,
    HttpStatusCode? StatusCode = null,
    string? Code = null)
{
    public static ApiResult<T> Success(T? data) => new(true, data);

    public static ApiResult<T> Failure(
        string message,
        HttpStatusCode? statusCode = null,
        string? code = null) => new(false, default, message, statusCode, code);
}
