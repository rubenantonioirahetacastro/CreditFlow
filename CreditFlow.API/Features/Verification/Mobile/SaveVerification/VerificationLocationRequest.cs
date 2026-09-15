using System.ComponentModel.DataAnnotations;

namespace CreditFlow.API.Features.Verification.Mobile.SaveVerification;

public sealed class VerificationLocationRequest
{
    [Required(ErrorMessage = "La latitud es obligatoria.")]
    [Range(typeof(decimal), "-90", "90", ErrorMessage = "La latitud no es válida.")]
    public decimal? NLatitud { get; init; }

    [Required(ErrorMessage = "La longitud es obligatoria.")]
    [Range(typeof(decimal), "-180", "180", ErrorMessage = "La longitud no es válida.")]
    public decimal? NLongitud { get; init; }
}
