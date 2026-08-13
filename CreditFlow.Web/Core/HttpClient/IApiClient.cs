namespace CreditFlow.Web.Core.Http;


public interface IApiClient
{
    /// <summary>GET que nunca lanza: si falla o no hay éxito, devuelve default (ej. null).</summary>
    Task<T?> GetAsync<T>(string url);

    /// <summary>POST/PUT que informa si se pudo o no, con el mensaje de error del servidor si falló.</summary>
    Task<(bool Exito, string? Mensaje)> PutAsync(string url, object? body = null);

    Task<(bool Exito, string? Mensaje)> PostAsync(string url, object? body = null);
}
