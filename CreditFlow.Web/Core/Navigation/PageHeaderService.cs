namespace CreditFlow.Web.Core.Navigation;

public class PageHeaderService
{
    private object? owner;

    public string? Title { get; private set; }
    public string? Description { get; private set; }
    public string? Icon { get; private set; }

    /// <summary>Niveles superiores de la ruta (sin incluir la página actual), p. ej. Otorgamiento / Procesamiento de Datos.</summary>
    public IReadOnlyList<PagePathItem> Path { get; private set; } = [];

    /// <summary>Si la ruta comienza con «Inicio» (enlace a Home). Falso en la propia Home.</summary>
    public bool ShowHome { get; private set; }

    public event Action? Changed;

    public void Set(string? title) => Set(null, title);

    /// <summary>
    /// Publica el encabezado de la barra superior en nombre de <paramref name="source"/>.
    /// </summary>
    public void Set(
        object? source,
        string? title,
        string? description = null,
        string? icon = null,
        IReadOnlyList<PagePathItem>? path = null,
        bool showHome = false)
    {
        owner = source;
        Title = title;
        Description = description;
        Icon = icon;
        Path = path ?? [];
        ShowHome = showHome;
        Changed?.Invoke();
    }

    public void Clear() => Set(null, null);

    /// <summary>
    /// Limpia el encabezado solo si todavía pertenece a <paramref name="source"/>. Evita que la página
    /// que se desecha al navegar borre el encabezado que la página nueva ya publicó.
    /// </summary>
    public void Clear(object source)
    {
        if (ReferenceEquals(owner, source))
            Clear();
    }
}
