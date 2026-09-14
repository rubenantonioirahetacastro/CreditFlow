namespace CreditFlow.API.Application.DTOs
{
    // Equivalente al resultado de [ESV].[ObtenerPrimLineaCred].
    public record PrimLineaCreditoResult(
        int NCodLinea,
        string CDescLinea,
        decimal NTasaCom,
        decimal NTasaComision);
}
