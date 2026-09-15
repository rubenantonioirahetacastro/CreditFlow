using Microsoft.AspNetCore.Diagnostics;

namespace CreditFlow.API.Core.Errors;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested)
            return false;

        var (statusCode, code, message) = exception switch
        {
            AppException appException => (
                (int)appException.StatusCode,
                appException.Code,
                appException.Message),
            _ => (
                StatusCodes.Status500InternalServerError,
                "internal_error",
                "Ocurrió un error interno. Intentá nuevamente más tarde.")
        };

        if (exception is AppException)
        {
            logger.LogWarning(
                "Solicitud rechazada en {Method} {Path}. Código: {Code}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                code);
        }
        else
        {
            logger.LogError(
                exception,
                "Error procesando {Method} {Path}. Código: {Code}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                code);
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(
            new ApiErrorResponse(code, message),
            cancellationToken);

        return true;
    }
}
