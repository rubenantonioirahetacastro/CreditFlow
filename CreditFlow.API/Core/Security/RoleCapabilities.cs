namespace CreditFlow.API.Core.Security;

public static class RoleCapabilities
{
    private static readonly string[] ClientSet =
    [
        Capabilities.CreditRequest,
        Capabilities.Simulate,
        Capabilities.Profile,
    ];

    private static readonly string[] FieldStaffSet =
    [
        Capabilities.Simulate,
        Capabilities.Prospect,
        Capabilities.Verify,
        Capabilities.AssignedClients,
        Capabilities.OfflineSync,
        Capabilities.Profile,
    ];

    private static readonly string[] ProspectorSet =
    [
        Capabilities.Simulate,
        Capabilities.Prospect,
        Capabilities.OfflineSync,
        Capabilities.Profile,
    ];

    // Cashier conserva su acceso al login móvil, sin verificación (como antes de unificar las listas).
    private static readonly string[] CashierSet = [Capabilities.Simulate];

    private static readonly IReadOnlyDictionary<int, string[]> ByRole = new Dictionary<int, string[]>
    {
        [RoleIds.Client] = ClientSet,
        [RoleIds.Verifier] = FieldStaffSet,
        [RoleIds.Administrator] = FieldStaffSet,
        [RoleIds.Supervisor] = FieldStaffSet,
        [RoleIds.CreditOfficer] = ProspectorSet,
        [RoleIds.Cashier] = CashierSet,
        [RoleIds.Technology] = FieldStaffSet,
    };

    // Acceso al login móvil: cualquier rol con al menos una capacidad.
    public static readonly int[] MobileAccess = ByRole
        .Where(item => item.Value.Length > 0)
        .Select(item => item.Key)
        .ToArray();

    // Acceso a datos comunes de empleados móviles, sin conceder operaciones de verificación.
    public static readonly int[] EmployeeAccess = RolesWithAny(
        Capabilities.Prospect,
        Capabilities.Verify,
        Capabilities.AssignedClients);

    // Acceso a los endpoints de verificación: roles con la capacidad "verify".
    public static readonly int[] VerificationAccess = RolesWith(Capabilities.Verify);

    public static IReadOnlyList<string> For(IEnumerable<int> roleIds) => roleIds
        .Where(ByRole.ContainsKey)
        .SelectMany(roleId => ByRole[roleId])
        .Distinct()
        .ToList();

    private static int[] RolesWith(string capability) => ByRole
        .Where(item => item.Value.Contains(capability))
        .Select(item => item.Key)
        .ToArray();

    private static int[] RolesWithAny(params string[] capabilities) => ByRole
        .Where(item => item.Value.Any(capabilities.Contains))
        .Select(item => item.Key)
        .ToArray();
}
