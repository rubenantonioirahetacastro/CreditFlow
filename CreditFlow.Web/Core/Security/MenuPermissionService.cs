using CreditFlow.Web.Core.Http;
using CreditFlow.Web.Core.Navigation;

namespace CreditFlow.Web.Core.Security;

/// <summary>
/// Permisos de menú del usuario autenticado (suma de sus roles, configurada en «Permisos por rol»).
/// Se cargan una vez por circuito con <see cref="CargarAsync"/>; después el menú lateral, la protección
/// de páginas y los botones de cada pantalla los consultan sin esperar. La API conserva sus propias
/// políticas por rol como límite superior.
/// </summary>
public sealed class MenuPermissionService(IApiClient apiClient)
{
    private Task? carga;
    private Dictionary<string, MenuPermission> permisos = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Se dispara al recargar los permisos (p. ej. tras cambiar los del propio rol).</summary>
    public event Action? Changed;

    public bool Cargado { get; private set; }

    /// <summary>Verdadero si no se pudieron cargar; el menú muestra un aviso en lugar de quedar vacío sin explicación.</summary>
    public bool ErrorDeCarga { get; private set; }

    public Task CargarAsync() => carga ??= CargarDesdeApiAsync();

    public async Task RecargarAsync()
    {
        carga = CargarDesdeApiAsync();
        await carga;
        Changed?.Invoke();
    }

    /// <summary>Si el usuario puede ver la página o, para un grupo o sección, alguna de sus páginas.</summary>
    public bool PuedeVer(MenuNode nodo) =>
        nodo.Pages().Any(page => permisos.TryGetValue(page.Key, out var p) && p.Ver);

    /// <summary>Si el usuario puede hacer la acción en la pantalla con esa clave (None = ver).</summary>
    public bool Puede(string clave, MenuActions accion = MenuActions.None)
    {
        if (!permisos.TryGetValue(clave, out var p))
            return false;

        return accion switch
        {
            MenuActions.None => p.Ver,
            MenuActions.Crear => p.Crear,
            MenuActions.Editar => p.Editar,
            MenuActions.Eliminar => p.Eliminar,
            _ => false
        };
    }

    private async Task CargarDesdeApiAsync()
    {
        var result = await apiClient.GetAsync<List<MenuPermission>>(
            "api/roles/mis-permisos",
            "No se pudieron cargar los permisos del menú.");

        ErrorDeCarga = !result.IsSuccess;
        if (result.IsSuccess)
            permisos = (result.Data ?? []).ToDictionary(p => p.Clave, StringComparer.OrdinalIgnoreCase);

        Cargado = true;
    }
}
