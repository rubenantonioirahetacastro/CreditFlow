using System.ComponentModel.DataAnnotations;

namespace CreditFlow.API.Features.Payment.Mobile.RegisterPayment;

public class RegisterPaymentRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "La agencia es obligatoria.")]
    public int NCodAge { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El crédito es obligatorio.")]
    public int NCodCred { get; set; }

    [Range(typeof(decimal), "0.01", "999999999", ErrorMessage = "El monto a abonar debe ser mayor que cero.")]
    public decimal MontoAbonado { get; set; }
}
