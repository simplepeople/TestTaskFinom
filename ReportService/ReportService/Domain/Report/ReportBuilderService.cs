using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReportService.Domain.Empl;
using ReportService.Domain.Report.Models;

namespace ReportService.Domain.Report
{
    internal sealed class ReportBuilderService : IReportBuilderService
    {
        private readonly IEmployeeService employeeService;
        private readonly IReportFormatter reportFormatter;

        public ReportBuilderService(IEmployeeService employeeService, IReportFormatter reportFormatter)
        {
            this.employeeService = employeeService;
            this.reportFormatter = reportFormatter;
        }

        /// <summary>
        /// Создает сводный отчет по последним зарплатам сотрудников по департаментам
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public async Task<string> CreateReport(int year, int month)
        {
            //todo: ключевое здесь - по последним, т.к. данные по зарплате сотрудника вытаскиваются через сторонний
            //сервис, в котором даты не указываются

            //todo вероятно стоит добавить кеширование полных или промежуточных результатов,
            //но нет точных предпосылок о сроках валидности данных

            var employees= await this.employeeService.GetAll();
            var reportData = GenerateReportData(year, month, employees);
            return this.reportFormatter.CreateReport(reportData);
        }

        private ReportData GenerateReportData(int year, int month, IEnumerable<Empl.Models.Employee> employees)
        {
            var departments = employees.GroupBy(x => x.Department, e => new Report.Models.Employee
            {
                Name = e.Name,
                Salary = e.Salary,
            }).Select(g => new Department
            {
                Name = g.Key,
                TotalSalary = g.Sum(re => re.Salary),
                Employees = g.ToList(),
            }).ToList();
            var company = new Company
            {
                TotalSalary = departments.Sum(d => d.TotalSalary),
                Departments = departments,
            };
            var reportData = new ReportData
            {
                Year = year,
                Month = month,
                Company = company,
            };
            return reportData;
        }
    }
}