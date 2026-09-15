namespace CreditFlow.API.Features.Simulator.Shared.SimulateCalendar
{
    // Equivalente a DevuelveVarNegocio(codigo) del VB de producción: parámetros de
    // negocio configurables (tabla VerNegocio: nCodVar/cNomVar/cValorVar/nTipoVar).
    public interface IBusinessVariableService
    {
        Task<string?> GetValueAsync(int variableCode);

        Task<int> GetIntAsync(int variableCode, int defaultValue);

        Task<decimal> GetDecimalAsync(int variableCode, decimal defaultValue);
    }
}
