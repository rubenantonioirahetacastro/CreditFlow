namespace CreditFlow.API.Features.Credit.Web.ManageCreditLines
{
    public interface ICreditLineManagementService
    {
        Task<List<CreditLineDto>> GetAllAsync();

        Task<CreditLineDto?> GetByIdAsync(int id);

        Task<CreditLineDto> CreateAsync(CreateCreditLineRequest request, string? user);

        Task<CreditLineDto?> UpdateAsync(int id, UpdateCreditLineRequest request, string? user);

        Task<bool> DeleteAsync(int id);
    }
}
