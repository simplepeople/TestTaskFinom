using System.Collections.Generic;
using System.Threading.Tasks;
using ReportService.Domain.Empl.Reader.Models;

namespace ReportService.Domain.Empl.Reader
{
    internal interface IEmployeeReader
    {
        Task<List<EmployeeDb>> ReadEmployeesFromDb();
    }
}