namespace CreditFlow.API.Features.Credit.Shared.CreditLine
{
    // Equivalente a clsCredProcesos_SV.ObtenerPrimLineaCredSV / [ESV].[ObtenerPrimLineaCred]:
    // resuelve la línea de crédito aplicable (por agencia, campaña, categoría, moneda,
    // plazo y monto) y la tasa de comisión del tarifario (CredGastos).
    public interface IPrimaryCreditLineService
    {
        Task<PrimaryCreditLineResult> ResolveAsync(PrimaryCreditLineRequest request);
    }
}
