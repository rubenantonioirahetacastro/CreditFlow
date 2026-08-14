namespace CreditFlow.API.Features.Mantenimientos.LineasCredito.Requests
{
    // NCodLinea no se incluye: es autogenerado por la base de datos (identity).
    // CUser no se incluye: se asigna en el servidor con el usuario autenticado.
    public class CrearLineaCreditoRequest
    {
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
    }
}
