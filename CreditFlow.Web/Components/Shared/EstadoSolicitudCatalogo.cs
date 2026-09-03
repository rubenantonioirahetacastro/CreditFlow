namespace CreditFlow.Web.Components.Shared;

public record EstadoColor(string Color, string ColorFondo);

public static class EstadoSolicitudCatalogo
{
    private static readonly Dictionary<int, EstadoColor> Colores = new()
    {
        [1] = new("#B45309", "#FEF3C7"), // Solicitado
        [2] = new("#C2410C", "#FFEDD5"), // En Analisís
        [3] = new("#1D4ED8", "#DBEAFE"), // Aprobado
        [4] = new("#15803D", "#DCFCE7"), // Desembolsado
        [5] = new("#0E7490", "#CFFAFE"), // Verificado
        [30] = new("#4D7C0F", "#ECFCCB"), // Vigente
        [50] = new("#B91C1C", "#FEE2E2"), // Cancelado
    };
    
    private static readonly EstadoColor Default = new("#374151", "#E5E7EB");

    public static EstadoColor Resolver(int nValor) => Colores.GetValueOrDefault(nValor, Default);
}
