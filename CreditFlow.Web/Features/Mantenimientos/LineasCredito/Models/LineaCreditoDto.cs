namespace CreditFlow.Web.Models.Mantenimientos;

public class LineaCreditoDto
{
    public int NCodLinea { get; set; }

    public string Descripcion { get; set; } = string.Empty;

    public decimal TasaComision { get; set; }

    public int Producto { get; set; }

    public int SubProducto { get; set; }

    public int PlazoMinimo { get; set; }

    public int PlazoMaximo { get; set; }

    public decimal MontoMinimo { get; set; }

    public decimal MontoMaximo { get; set; }

    public int? NumeroPrestamosMinimo { get; set; }

    public int? NumeroPrestamosMaximo { get; set; }

    public bool? AplicaRefinanciamiento { get; set; }

    public string? Usuario { get; set; }

    public bool Activa { get; set; }
}
