using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Credit.Shared.Calendar
{
    public class HolidayService(DbNegocioContext context) : IHolidayService
    {
        public async Task<List<DateTime>> GetHolidaysAsync(DateTime disbursementDate, int agencyCode)
        {
            var feriados = await(
             from cf in context.CredFeriados
             join cfa in context.CredFeriadoAges
                 on cf.NIdFeriado equals cfa.NIdFeriado
             where cf.BEstado == true
                   && cfa.NCodAge == agencyCode
                   && cf.DFecha >= disbursementDate.Date
             select cf.DFecha
             ).ToListAsync();
            return feriados;
        }
    }
}
