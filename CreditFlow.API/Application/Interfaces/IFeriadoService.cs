namespace CreditFlow.API.Application.Interfaces
{
    public interface IFeriadoService
    {
        Task<List<DateTime>> ObtenerFeriadosAsync(DateTime fechaDesembolso, int codigoAgencia);
    }
}
