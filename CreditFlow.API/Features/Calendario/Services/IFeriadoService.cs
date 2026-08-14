namespace CreditFlow.API.Features.Calendario.Services
{
    public interface IFeriadoService
    {
        Task<List<DateTime>> ObtenerFeriadosAsync(DateTime fechaDesembolso, int codigoAgencia);
    }
}
