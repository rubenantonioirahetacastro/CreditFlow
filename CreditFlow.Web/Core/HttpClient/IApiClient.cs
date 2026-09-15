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

    Task<ApiResult> PostAsync(
        string url,
        HttpContent content,
        string? fallbackMessage = null,
        CancellationToken cancellationToken = default);

    Task<ApiResult> PutAsync<TRequest>(
        string url,
        TRequest body,
        string? fallbackMessage = null,
        CancellationToken cancellationToken = default);

    Task<ApiResult> PutAsync(
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
