using CreditFlow.Web.Core.Security;

namespace CreditFlow.Web.Core.Navigation;

/// <summary>Acciones que se pueden permitir en una opción del menú, además de verla.</summary>
[Flags]
public enum MenuActions
{
    None = 0,
    Crear = 1,
    Editar = 2,
    Eliminar = 4
}

/// <summary>Cómo se dibuja un nodo en el menú lateral.</summary>
public enum MenuNodeKind
{
    /// <summary>Opción con página (enlace).</summary>
    Page,

    /// <summary>Grupo que se despliega y pliega.</summary>
    Group,

    /// <summary>Encabezado fijo de sección, siempre desplegado (p. ej. «Otorgamiento»).</summary>
    Section
}

/// <summary>
/// Opción del menú. Solo las páginas (hojas) tienen permisos propios; un grupo se ve si alguna de sus
/// páginas se ve. La <see cref="Key"/> es estable: se guarda en la base de datos (RolMenuPermiso).
/// </summary>
public sealed record MenuNode(
    string Key,
    string Title,
    string Icon,
    MenuNodeKind Kind,
    string? Href = null,
    MenuActions Actions = MenuActions.None,
    IReadOnlyCollection<int>? ApiRoleIds = null,
    IReadOnlyCollection<int>? ApiActionRoleIds = null,
    IReadOnlyList<MenuNode>? Children = null)
{
    public IReadOnlyList<MenuNode> Items => Children ?? [];

    public bool IsPage => Kind == MenuNodeKind.Page;

    /// <summary>Roles a los que la API permite usar esta pantalla (null = todos). Fuera de ellos no se puede otorgar.</summary>
    public bool ApiAllowsView(int roleId) => ApiRoleIds is null || ApiRoleIds.Contains(roleId);

    /// <summary>Roles a los que la API permite crear/editar/eliminar en esta pantalla (null = los mismos que pueden verla).</summary>
    public bool ApiAllowsActions(int roleId) => ApiAllowsView(roleId) && (ApiActionRoleIds is null || ApiActionRoleIds.Contains(roleId));

    public IEnumerable<MenuNode> Pages() => IsPage ? [this] : Items.SelectMany(child => child.Pages());
}

/// <summary>
/// Árbol de menús y submenús de la Web: fuente única para el menú lateral, el mantenimiento «Permisos por rol»
/// y la protección de páginas. Para agregar una pantalla se agrega aquí su nodo con una clave nueva.
/// </summary>
public static class MenuCatalog
{
    public const string RolePermissionsKey = "mantenimientos.permisos";

    public static readonly IReadOnlyList<MenuNode> Tree =
    [
        new("home", "Home", "home", MenuNodeKind.Page, ""),
        new("simulador", "Simulador de Crédito", "calculate", MenuNodeKind.Page, "simulador-credito",
            ApiRoleIds: RoleIds.CalendarConfigurationAccess),
        new("otorgamiento", "Otorgamiento", "credit_score", MenuNodeKind.Section, Children:
        [
            new("otorgamiento.procesamiento", "Procesamiento de Datos", "fact_check", MenuNodeKind.Group, Children:
            [
                new("otorgamiento.procesamiento.verificacion", "Bandeja de verificación", "inbox", MenuNodeKind.Page,
                    "otorgamiento/procesamiento-datos/bandeja-verificacion"),
                new("otorgamiento.procesamiento.readecuacion", "Bandeja de crédito readecuación", "restart_alt", MenuNodeKind.Page,
                    "otorgamiento/procesamiento-datos/bandeja-credito-readecuacion")
            ]),
            new("otorgamiento.autorizacion", "Niveles de Autorización", "verified_user", MenuNodeKind.Group, Children:
            [
                new("otorgamiento.autorizacion.autorizar-credito", "Autorizar crédito", "task_alt", MenuNodeKind.Page,
                    "otorgamiento/niveles-autorizacion/autorizar-credito")
            ]),
            new("otorgamiento.tesoreria", "Tesorería", "account_balance", MenuNodeKind.Group, Children:
            [
                new("otorgamiento.tesoreria.abonar", "Bandeja de créditos a Abonar", "payments", MenuNodeKind.Page,
                    "otorgamiento/tesoreria/bandeja-creditos-abonar"),
                new("otorgamiento.tesoreria.abonar-agencia", "Bandeja de créditos a abonar en agencia", "store", MenuNodeKind.Page,
                    "otorgamiento/tesoreria/bandeja-creditos-abonar-agencia")
            ]),
            new("otorgamiento.agencia", "Agencia Sucursal", "storefront", MenuNodeKind.Group, Children:
            [
                new("otorgamiento.agencia.registrar-oficiales", "Registrar créditos a oficiales", "person_add", MenuNodeKind.Page,
                    "otorgamiento/agencia-sucursal/registrar-creditos-oficiales"),
                new("otorgamiento.agencia.abono-oficial", "Abono a oficial de desembolso", "paid", MenuNodeKind.Page,
                    "otorgamiento/agencia-sucursal/abono-oficial-desembolso")
            ]),
            new("otorgamiento.desembolso", "Desembolso", "local_atm", MenuNodeKind.Group, Children:
            [
                new("otorgamiento.desembolso.bandeja", "Bandeja de créditos por desembolsar", "outbox", MenuNodeKind.Page,
                    "otorgamiento/desembolso/bandeja-creditos-desembolsar")
            ])
        ]),
        new("mantenimientos", "Mantenimientos", "build", MenuNodeKind.Group, Children:
        [
            new("mantenimientos.empleados", "Empleados", "badge", MenuNodeKind.Page, "empleados",
                MenuActions.Crear | MenuActions.Editar, RoleIds.MaintenanceAccess, RoleIds.MaintenanceEditAccess),
            new("mantenimientos.roles", "Roles", "admin_panel_settings", MenuNodeKind.Page, "roles",
                MenuActions.Crear | MenuActions.Editar, RoleIds.MaintenanceAccess),
            new(RolePermissionsKey, "Permisos por rol", "lock_person", MenuNodeKind.Page, "permisos-roles",
                MenuActions.Editar, RoleIds.MaintenanceAccess, RoleIds.MaintenanceEditAccess),
            new("mantenimientos.agencias", "Agencias", "storefront", MenuNodeKind.Page, "agencias",
                MenuActions.Crear | MenuActions.Editar | MenuActions.Eliminar, RoleIds.MaintenanceAccess, RoleIds.MaintenanceEditAccess),
            new("mantenimientos.catalogos", "Catálogos", "list_alt", MenuNodeKind.Page, "catalogos-codigos",
                MenuActions.Crear | MenuActions.Editar | MenuActions.Eliminar, RoleIds.MaintenanceAccess),
            new("mantenimientos.lineas", "Líneas de Crédito", "credit_card", MenuNodeKind.Page, "lineas-credito",
                MenuActions.Crear | MenuActions.Editar | MenuActions.Eliminar, RoleIds.MaintenanceAccess, RoleIds.MaintenanceEditAccess)
        ])
    ];

    public static IEnumerable<MenuNode> Pages => Tree.SelectMany(node => node.Pages());

    /// <summary>Página del menú a la que corresponde una ruta relativa («empleados», «otorgamiento/…»); null si no es del menú.</summary>
    public static MenuNode? FindPageByPath(string relativePath)
    {
        var path = relativePath.Split('?', '#')[0].Trim('/');
        return Pages.FirstOrDefault(page => string.Equals(page.Href?.Trim('/'), path, StringComparison.OrdinalIgnoreCase));
    }
}
