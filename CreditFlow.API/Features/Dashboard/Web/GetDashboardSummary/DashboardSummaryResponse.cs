namespace CreditFlow.API.Features.Dashboard.Web.GetDashboardSummary;

public class DashboardKpis
{
    public decimal CarteraVigente { get; set; }
    public decimal MontoColocadoTotal { get; set; }
    public int CreditosActivos { get; set; }
    public int CreditosTotales { get; set; }
    public decimal CarteraEnRiesgo { get; set; }
    public decimal PorcentajeMorosidad { get; set; }
    public decimal TasaPromedioPonderada { get; set; }
    public int CreditosUltimos30Dias { get; set; }
    public decimal MontoUltimos30Dias { get; set; }
}

public class SerieMensualItem
{
    public string Periodo { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public int Cantidad { get; set; }
}

public class DistribucionItem
{
    public string Etiqueta { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal Monto { get; set; }
}

public class RangoMoraItem
{
    public string Rango { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal Monto { get; set; }
}

public class TopCreditoItem
{
    public int NCodAge { get; set; }
    public int NCodCred { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string? Estado { get; set; }
    public string? Agencia { get; set; }
}

public class DashboardSummaryResponse
{
    public DashboardKpis Kpis { get; set; } = new();
    public List<SerieMensualItem> ColocacionMensual { get; set; } = new();
    public List<DistribucionItem> PorEstado { get; set; } = new();
    public List<DistribucionItem> PorSubProducto { get; set; } = new();
    public List<DistribucionItem> PorAgencia { get; set; } = new();
    public List<RangoMoraItem> DistribucionMora { get; set; } = new();
    public List<TopCreditoItem> TopCreditos { get; set; } = new();
}
