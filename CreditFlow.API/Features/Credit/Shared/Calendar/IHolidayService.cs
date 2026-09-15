namespace CreditFlow.API.Features.Credit.Shared.Calendar
{
    public interface IHolidayService
    {
        Task<List<DateTime>> GetHolidaysAsync(DateTime disbursementDate, int agencyCode);
    }
}
