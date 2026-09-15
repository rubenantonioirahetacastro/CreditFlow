namespace CreditFlow.API.Features.Credit.Shared.CreditLine
{
    // Equivalente al resultado de [ESV].[ObtenerPrimLineaCred].
    public record PrimaryCreditLineResult(
        int NCodLinea,
        string CDescLinea,
        decimal NTasaCom,
        decimal NTasaComision);
}
