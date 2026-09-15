using System.ComponentModel.DataAnnotations;

namespace CreditFlow.API.Features.Credit.Web.ManageCreditLines
{
    // NCodLinea no se incluye: es autogenerado por la base de datos (identity).
    // CUser no se incluye: se asigna en el servidor con el usuario autenticado.
    public class CreateCreditLineRequest
    {
        [Required(ErrorMessage = "La descripción de la línea es requerida.")]
        [StringLength(150, ErrorMessage = "La descripción no puede superar los 150 caracteres.")]
        public string Descripcion { get; set; } = string.Empty;

        [Range(typeof(decimal), "0", "999999999", ErrorMessage = "La tasa de comisión no puede ser negativa.")]
        public decimal TasaComision { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El producto es obligatorio.")]
        public int Producto { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El subproducto es obligatorio.")]
        public int SubProducto { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El plazo mínimo debe ser mayor que cero.")]
        public int PlazoMinimo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El plazo máximo debe ser mayor que cero.")]
        public int PlazoMaximo { get; set; }

        [Range(typeof(decimal), "0.01", "999999999", ErrorMessage = "El monto mínimo debe ser mayor que cero.")]
        public decimal MontoMinimo { get; set; }

        [Range(typeof(decimal), "0.01", "999999999", ErrorMessage = "El monto máximo debe ser mayor que cero.")]
        public decimal MontoMaximo { get; set; }

        public int? NumeroPrestamosMinimo { get; set; }

        public int? NumeroPrestamosMaximo { get; set; }

        public bool? AplicaRefinanciamiento { get; set; }
    }
}
