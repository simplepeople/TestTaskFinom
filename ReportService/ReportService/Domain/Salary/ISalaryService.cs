using System.Threading.Tasks;

namespace ReportService.Domain.Salary
{
    internal interface ISalaryService
    {
        Task<int> GetEmployeeSalaryByInnBuh(string inn, string employeeBuhCode);
    }
}