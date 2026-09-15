namespace CreditFlow.API.Features.Agency.Web.ManageAgencies;

public interface IAgencyService
{
    Task<IReadOnlyList<AgencyDto>> GetAllAsync();
    Task<AgencyDto?> GetByIdAsync(int id);
    Task<AgencyDto> CreateAsync(CreateAgencyRequest request);
    Task<AgencyDto?> UpdateAsync(int id, UpdateAgencyRequest request);
    Task<bool> DeleteAsync(int id);
}
