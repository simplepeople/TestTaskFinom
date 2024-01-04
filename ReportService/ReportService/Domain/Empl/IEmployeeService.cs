using ReportService.Domain.Empl.Models;

namespace ReportService.Domain.Empl
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAll();
    }
}
