namespace CreditFlow.Web.Core.Http;

public interface IApiClient
{
    Task<ApiResult<T>> GetAsync<T>(
        string url,
        string? fallbackMessage = null,
        CancellationToken cancellationToken = default);

    Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(
        string url,
        TRequest body,
        string? fallbackMessage = null,
        CancellationToken cancellationToken = default);

    Task<ApiResult<TResponse>> PostAnonymousAsync<TRequest, TResponse>(
        string url,
        TRequest body,
        string? fallbackMessage = null,
        CancellationToken cancellationToken = default);

    Task<ApiResult> PostAsync<TRequest>(
        string url,
        TRequest body,
        string? fallbackMessage = null,
        CancellationToken cancellationToken = default);

    // Nombre distinto a propósito: si se llamara PostAsync, una sobrecarga genérica
    // TRequest con coincidencia exacta de tipo le gana a esta en la resolución de C#,
    // y el content (multipart, etc.) terminaría serializado como JSON por error.
    Task<ApiResult> PostFormAsync(
        string url,
        HttpContent content,
        string? fallbackMessage = null,
        CancellationToken cancellationToken = default);

    Task<ApiResult> PutAsync<TRequest>(
        string url,
        TRequest body,
        string? fallbackMessage = null,
        CancellationToken cancellationToken = default);

    Task<ApiResult> PutFormAsync(
        string url,
        HttpContent content,
        string? fallbackMessage = null,
        CancellationToken cancellationToken = default);

    Task<ApiResult> DeleteAsync(
        string url,
        string? fallbackMessage = null,
        CancellationToken cancellationToken = default);

    Task<ApiResult<string>> GetImageDataUrlAsync(
        string url,
        string? fallbackMessage = null,
        CancellationToken cancellationToken = default);
}
