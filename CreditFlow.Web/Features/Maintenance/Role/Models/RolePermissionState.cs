using CreditFlow.Web.Core.Navigation;
using CreditFlow.Web.Core.Security;

namespace CreditFlow.Web.Features.Maintenance.Role.Models;

/// <summary>
/// Permisos de un rol mientras se editan en «Permisos por rol». Reglas: crear/editar/eliminar implican ver;
/// quitar ver quita las acciones; solo se ofrecen las acciones que la pantalla admite y que la API
/// permite a ese rol (las demás quedan bloqueadas).
/// </summary>
public sealed class RolePermissionState
{
    private readonly Dictionary<string, MenuPermission> permisos;

    public RolePermissionState(int idRol, IEnumerable<MenuPermission> permisosGuardados)
    {
        IdRol = idRol;
        permisos = MenuCatalog.Pages.ToDictionary(p => p.Key, p => new MenuPermission { Clave = p.Key }, StringComparer.OrdinalIgnoreCase);

        foreach (var guardado in permisosGuardados)
        {
            if (permisos.TryGetValue(guardado.Clave, out var actual))
            {
                actual.Ver = guardado.Ver || guardado.Crear || guardado.Editar || guardado.Eliminar;
                actual.Crear = guardado.Crear;
                actual.Editar = guardado.Editar;
                actual.Eliminar = guardado.Eliminar;
            }
        }

        Original = Firma();
    }

    public int IdRol { get; }

    private string Original { get; set; }

    public bool HayCambios => Firma() != Original;

    /// <summary>«Permisos por rol» siempre queda completo para Administrador y Tecnología (lo garantiza también la API).</summary>
    public bool EsBloqueadoPorSeguridad(MenuNode pagina) =>
        pagina.Key == MenuCatalog.RolePermissionsKey && RoleIds.GlobalAdministrators.Contains(IdRol);

    public bool PuedeOtorgarVer(MenuNode pagina) => pagina.ApiAllowsView(IdRol) && !EsBloqueadoPorSeguridad(pagina);

    public bool PuedeOtorgar(MenuNode pagina, MenuActions accion) =>
        pagina.Actions.HasFlag(accion) && pagina.ApiAllowsActions(IdRol) && !EsBloqueadoPorSeguridad(pagina);

    public bool Tiene(MenuNode pagina, MenuActions accion = MenuActions.None)
    {
        var p = permisos[pagina.Key];
        return accion switch
        {
            MenuActions.None => p.Ver,
            MenuActions.Crear => p.Crear,
            MenuActions.Editar => p.Editar,
            MenuActions.Eliminar => p.Eliminar,
            _ => false
        };
    }

    public void Cambiar(MenuNode pagina, MenuActions accion, bool valor)
    {
        if (accion == MenuActions.None ? !PuedeOtorgarVer(pagina) : !PuedeOtorgar(pagina, accion))
            return;

        var p = permisos[pagina.Key];
        switch (accion)
        {
            case MenuActions.None:
                p.Ver = valor;
                if (!valor)
                    p.Crear = p.Editar = p.Eliminar = false;
                break;
            case MenuActions.Crear:
                p.Crear = valor;
                break;
            case MenuActions.Editar:
                p.Editar = valor;
                break;
            case MenuActions.Eliminar:
                p.Eliminar = valor;
                break;
        }

        if (valor)
            p.Ver = true;
    }

    /// <summary>Estado de un grupo (o página) para una acción: todas, ninguna o parcial (null), entre las otorgables.</summary>
    public bool? Estado(MenuNode nodo, MenuActions accion = MenuActions.None)
    {
        var aplicables = Aplicables(nodo, accion).ToList();
        if (aplicables.Count == 0)
            return false;

        var marcadas = aplicables.Count(p => Tiene(p, accion));
        return marcadas == 0 ? false : marcadas == aplicables.Count ? true : null;
    }

    /// <summary>Si alguna página del nodo admite la acción para este rol.</summary>
    public bool Aplica(MenuNode nodo, MenuActions accion = MenuActions.None) => Aplicables(nodo, accion).Any();

    public void CambiarTodo(MenuNode nodo, MenuActions accion, bool valor)
    {
        foreach (var pagina in Aplicables(nodo, accion))
            Cambiar(pagina, accion, valor);
    }

    public void CambiarTodo(bool valor)
    {
        foreach (var nodo in MenuCatalog.Tree)
        {
            if (valor)
            {
                // Marcar todo: ver y todas las acciones otorgables.
                foreach (var accion in new[] { MenuActions.None, MenuActions.Crear, MenuActions.Editar, MenuActions.Eliminar })
                    CambiarTodo(nodo, accion, true);
            }
            else
            {
                CambiarTodo(nodo, MenuActions.None, false);
            }
        }
    }

    /// <summary>Copia los permisos de otro rol, respetando lo que este rol puede recibir.</summary>
    public void CopiarDe(IEnumerable<MenuPermission> otros)
    {
        CambiarTodo(false);
        var mapa = otros.ToDictionary(p => p.Clave, StringComparer.OrdinalIgnoreCase);
        foreach (var pagina in MenuCatalog.Pages)
        {
            if (!mapa.TryGetValue(pagina.Key, out var otro))
                continue;

            if (otro.Ver) Cambiar(pagina, MenuActions.None, true);
            if (otro.Crear) Cambiar(pagina, MenuActions.Crear, true);
            if (otro.Editar) Cambiar(pagina, MenuActions.Editar, true);
            if (otro.Eliminar) Cambiar(pagina, MenuActions.Eliminar, true);
        }
    }

    public int OpcionesHabilitadas => permisos.Values.Count(p => p.Ver);

    public List<MenuPermission> Lista() => permisos.Values
        .Where(p => p.Ver)
        .Select(p => new MenuPermission { Clave = p.Clave, Ver = true, Crear = p.Crear, Editar = p.Editar, Eliminar = p.Eliminar })
        .ToList();

    /// <summary>Marca el estado actual como guardado.</summary>
    public void ConfirmarGuardado() => Original = Firma();

    private IEnumerable<MenuNode> Aplicables(MenuNode nodo, MenuActions accion) =>
        nodo.Pages().Where(p => accion == MenuActions.None ? PuedeOtorgarVer(p) : PuedeOtorgar(p, accion));

    private string Firma() => string.Join('|', permisos.Values
        .OrderBy(p => p.Clave)
        .Select(p => $"{p.Clave}:{(p.Ver ? 1 : 0)}{(p.Crear ? 1 : 0)}{(p.Editar ? 1 : 0)}{(p.Eliminar ? 1 : 0)}"));
}
