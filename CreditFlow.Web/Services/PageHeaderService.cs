namespace CreditFlow.Web.Services;

/// <summary>
/// Permite que una página reemplace el nombre de la app en la barra superior
/// por su propio título (y un subtítulo opcional) mientras está activa.
/// </summary>
public class PageHeaderService
{
    public string? Title { get; private set; }
    public string? Subtitle { get; private set; }

    public event Action? Changed;

    public void Set(string? title, string? subtitle = null)
    {
        Title = title;
        Subtitle = subtitle;
        Changed?.Invoke();
    }

    public void Clear() => Set(null, null);
}
