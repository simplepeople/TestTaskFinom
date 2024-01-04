using System.Collections.Generic;

namespace ReportService.Domain.Report.Models
{
    public class Company
    {
        public int TotalSalary { get; set; }
        public IEnumerable<Department> Departments { get; set; }
    }
}