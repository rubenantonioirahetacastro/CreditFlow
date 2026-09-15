using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CreditFlow.Web.Services;

namespace CreditFlow.Web.Core.Http;

public sealed class ApiClient : IApiClient
{
    private const string ConnectionMessage =
        "No se pudo conectar con el servidor. Intentá nuevamente más tarde.";
    private const string TimeoutMessage =
        "La solicitud tardó demasiado. Verificá tu conexión e intentá nuevamente.";
    private const string UnexpectedResponseMessage =
        "El servidor respondió de forma inesperada. Intentá nuevamente más tarde.";

    private readonly HttpClient _httpClient;
    private readonly CustomAuthStateProvider _authStateProvider;

    public ApiClient(
        IHttpClientFactory httpClientFactory,
        CustomAuthStateProvider authStateProvider)
    {
        _httpClient = httpClientFactory.CreateClient("CreditFlowApi");
        _authStateProvider = authStateProvider;
    }

    public Task<ApiResult<T>> GetAsync<T>(
        string url,
        string? fallbackMessage = null,
        CancellationToken cancellationToken = default) =>
        SendForDataAsync<T>(HttpMethod.Get, url, null, fallbackMessage, cancellationToken);

    public Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(
        string url,
        TRequest body,
        string? fallbackMessage = null,
        CancellationToken cancellationToken = default) =>
        SendForDataAsync<TResponse>(
            HttpMethod.Post,
            url,
            JsonContent.Create(body),
            fallbackMessage,
            cancellationToken,
            attachToken: true);

    public Task<ApiResult<TResponse>> PostAnonymousAsync<TRequest, TResponse>(
        string url,
        TRequest body,
        string? fallbackMessage = null,
        CancellationToken cancellationToken = default) =>
        SendForDataAsync<TResponse>(
            HttpMethod.Post,
            url,
            JsonContent.Create(body),
            fallbackMessage,
            cancellationToken,
            attachToken: false);

    public Task<ApiResult> PostAsync<TRequest>(
        string url,
        TRequest body,
        string? fallbackMessage = null,
        CancellationToken cancellationToken = default) =>
        SendAsync(
            HttpMethod.Post,
            url,
            JsonContent.Create(body),
            fallbackMessage,
            cancellationToken);

    public Task<ApiResult> PostAsync(
        string url,
        HttpContent content,
        string? fallbackMessage = null,
        CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Post, url, content, fallbackMessage, cancellationToken);

    public Task<ApiResult> PutAsync<TRequest>(
        string url,
        TRequest body,
        string? fallbackMessage = null,
        CancellationToken cancellationToken = default) =>
        SendAsync(
            HttpMethod.Put,
            url,
            JsonContent.Create(body),
            fallbackMessage,
            cancellationToken);

    public Task<ApiResult> PutAsync(
        string url,
        HttpContent content,
        string? fallbackMessage = null,
        CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Put, url, content, fallbackMessage, cancellationToken);

    public Task<ApiResult> DeleteAsync(
        string url,
        string? fallbackMessage = null,
        CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Delete, url, null, fallbackMessage, cancellationToken);

    public async Task<ApiResult<string>> GetImageDataUrlAsync(
        string url,
        string? fallbackMessage = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var request = await CreateRequestAsync(HttpMethod.Get, url, null);
            using var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await ApiErrorParser.ParseAsync(response, fallbackMessage, cancellationToken);
                return ApiResult<string>.Failure(error.Message!, error.StatusCode, error.Code);
            }

            var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/jpeg";
            return ApiResult<string>.Success(
                $"data:{contentType};base64,{Convert.ToBase64String(bytes)}");
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return ApiResult<string>.Failure(TimeoutMessage);
        }
        catch (HttpRequestException)
        {
            return ApiResult<string>.Failure(ConnectionMessage);
        }
    }

    private async Task<ApiResult<T>> SendForDataAsync<T>(
        HttpMethod method,
        string url,
        HttpContent? content,
        string? fallbackMessage,
        CancellationToken cancellationToken,
        bool attachToken = true)
    {
        try
        {
            using var request = await CreateRequestAsync(method, url, content, attachToken);
            using var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await ApiErrorParser.ParseAsync(response, fallbackMessage, cancellationToken);
                return ApiResult<T>.Failure(error.Message!, error.StatusCode, error.Code);
            }

            var data = await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
            return data is null
                ? ApiResult<T>.Failure(UnexpectedResponseMessage, response.StatusCode)
                : ApiResult<T>.Success(data);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return ApiResult<T>.Failure(TimeoutMessage);
        }
        catch (HttpRequestException)
        {
            return ApiResult<T>.Failure(ConnectionMessage);
        }
        catch (JsonException)
        {
            return ApiResult<T>.Failure(UnexpectedResponseMessage);
        }
    }

    private async Task<ApiResult> SendAsync(
        HttpMethod method,
        string url,
        HttpContent? content,
        string? fallbackMessage,
        CancellationToken cancellationToken)
    {
        try
        {
            using var request = await CreateRequestAsync(method, url, content);
            using var response = await _httpClient.SendAsync(request, cancellationToken);

            return response.IsSuccessStatusCode
                ? ApiResult.Success()
                : await ApiErrorParser.ParseAsync(response, fallbackMessage, cancellationToken);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return ApiResult.Failure(TimeoutMessage);
        }
        catch (HttpRequestException)
        {
            return ApiResult.Failure(ConnectionMessage);
        }
        catch (JsonException)
        {
            return ApiResult.Failure(UnexpectedResponseMessage);
        }
    }

    private async Task<HttpRequestMessage> CreateRequestAsync(
        HttpMethod method,
        string url,
        HttpContent? content,
        bool attachToken = true)
    {
        var request = new HttpRequestMessage(method, url) { Content = content };
        if (!attachToken)
            return request;

        var token = await _authStateProvider.ObtenerAccessTokenAsync();

        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return request;
    }
}
