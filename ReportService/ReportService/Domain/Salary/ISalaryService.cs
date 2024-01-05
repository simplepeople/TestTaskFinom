using System.Threading.Tasks;

namespace ReportService.Domain.Salary
{
    internal interface ISalaryService
    {
        Task<int> GetEmployeeSalary(string inn, string employeeBuhCode);
    }
}