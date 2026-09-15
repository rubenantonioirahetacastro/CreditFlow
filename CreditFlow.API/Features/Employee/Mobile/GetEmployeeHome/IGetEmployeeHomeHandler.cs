namespace CreditFlow.API.Features.Employee.Mobile.GetEmployeeHome;

public interface IGetEmployeeHomeHandler
{
    Task<EmployeeHomeDto?> ExecuteAsync(int employeeId, int roleId);
}
