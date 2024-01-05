using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReportService.Domain.Buh;
using ReportService.Domain.Empl.Models;
using ReportService.Domain.Empl.Reader;
using ReportService.Domain.Empl.Reader.Models;
using ReportService.Domain.Salary;

namespace ReportService.Domain.Empl
{
    /// <summary>
    /// Собирает информацию с разных эндпоинтов для создания отчета по сотрудникам, департаментам и их зарплатам
    /// </summary>
    internal sealed class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeReader employeeReader;
        private readonly IBuhService buhService;
        private readonly ISalaryService salaryService;

        public EmployeeService(IEmployeeReader employeeReader, IBuhService buhService, ISalaryService salaryService)
        {
            this.employeeReader = employeeReader;
            this.buhService = buhService;
            this.salaryService = salaryService;
        }

        public async Task<IEnumerable<Employee>> GetAll()
        {
            var employeesDb = await this.employeeReader.ReadEmployees();

            var employees = await this.GetEmployees(employeesDb);

            return employees;
        }

        private async Task<IEnumerable<Employee>> GetEmployees(IEnumerable<EmployeeDb> employeesDb)
        {
            //todo для улучшения производительности (при n сотрудников 2n запросов - многовато в целом)
            //с учетом тротлинга обоих сервисов можно попытаться отсылать запросы асинхронно пачками, а не последовательно по одному
            //или же добавить новые, более удобные для текущего кейса, контракты в сами сервисы
            var employees = new List<Employee>();
            foreach (var employeeDb in employeesDb)
            {
                var buhCode = await this.buhService.GetEmployeeBuhCode(employeeDb.Inn);
                var salary = await this.salaryService.GetEmployeeSalary(employeeDb.Inn, buhCode);
                employees.Add(new Employee
                {
                    Department = employeeDb.Department,
                    Name = employeeDb.Name,
                    Salary = salary,
                });
            }
            return employees;
        }
    }
}