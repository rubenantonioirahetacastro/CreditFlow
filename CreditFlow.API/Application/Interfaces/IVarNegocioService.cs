namespace CreditFlow.API.Application.Interfaces
{
    // Equivalente a DevuelveVarNegocio(codigo) del VB de producción: parámetros de
    // negocio configurables (tabla VerNegocio: nCodVar/cNomVar/cValorVar/nTipoVar).
    public interface IVarNegocioService
    {
        Task<string?> ObtenerValorAsync(int nCodVar);

        Task<int> ObtenerValorIntAsync(int nCodVar, int valorPorDefecto);

        Task<decimal> ObtenerValorDecimalAsync(int nCodVar, decimal valorPorDefecto);
    }
}
