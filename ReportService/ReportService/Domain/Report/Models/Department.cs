using System.Collections.Generic;

namespace ReportService.Domain.Report.Models
{
    public class Department
    {
        public string Name { get; set; }

        public int TotalSalary { get; set; }

        public IEnumerable<Employee> Employees { get; set; }
    }
}