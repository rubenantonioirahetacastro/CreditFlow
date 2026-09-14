using CreditFlow.API.Application.DTOs;
using CreditFlow.API.Application.Requests;

namespace CreditFlow.API.Application.Interfaces
{
    // Equivalente a clsCredProcesos_SV.ObtenerPrimLineaCredSV / [ESV].[ObtenerPrimLineaCred]:
    // resuelve la línea de crédito aplicable (por agencia, campaña, categoría, moneda,
    // plazo y monto) y la tasa de comisión del tarifario (CredGastos).
    public interface IPrimLineaCreditoService
    {
        Task<PrimLineaCreditoResult> ObtenerPrimLineaCredAsync(PrimLineaCreditoRequest request);
    }
}
