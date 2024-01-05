using System.Collections.Generic;
using System.Threading.Tasks;
using ReportService.Domain.Empl.Models;

namespace ReportService.Domain.Empl
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAll();
    }
}
