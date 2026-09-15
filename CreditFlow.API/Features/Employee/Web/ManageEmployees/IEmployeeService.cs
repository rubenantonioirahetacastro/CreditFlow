namespace CreditFlow.API.Features.Employee.Web.ManageEmployees
{
    public interface IEmployeeService
    {
        Task<List<EmployeeDto>> GetAllAsync();

        Task<EmployeeDto> CreateAsync(CreateEmployeeRequest request);

        Task<EmployeeDto?> UpdateAsync(int id, UpdateEmployeeRequest request);

        Task<bool> DeleteAsync(int id);

        Task<(Stream Stream, string ContentType)?> GetPhotoAsync(int id);
    }
}
