using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using ReportService.Domain.Buh;
using ReportService.Domain.Empl.Models;
using ReportService.Domain.Salary;

namespace ReportService.Domain.Empl
{
    /// <summary>
    /// Собирает информацию с разных эндпоинтов для создания отчета по сотрудникам, департаментам и их зарплатам
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly SalaryService salaryService;
        private readonly BuhService buhService;

        public EmployeeService(SalaryService salaryService, BuhService buhService)
        {
            this.salaryService = salaryService;
            this.buhService = buhService;
        }

        public async Task<IEnumerable<Employee>> GetAll()
        {
            var employees = await this.ReadEmployeesFromDb();

            await this.FillEmployeesSalary(employees);

            return employees;
        }

        private async Task<List<Employee>> ReadEmployeesFromDb()
        {
            var employees = new List<Employee>();

            //todo вынести в конфиг или Vault
            //если будет >1 обращения к базе, то все манипуляции с запросами стоит вынести в отдельный слой
            const string connString = "Host=192.168.99.100;Username=postgres;Password=1;Database=employee";

            using (var connection = new NpgsqlConnection(connString))
            {
                connection.Open();

                //todo мне сложно представить, что в таблице emps данные о сотрудниках могут дублироваться
                //todo также сложно представить, что FK на deps может быть невалидным
                //поэтому не вижу здесь смысла делать LEFT join, также как и отдельный запрос в deps
                using (var command = new NpgsqlCommand("SELECT e.name, e.inn, d.name from emps e join deps d on e.departmentid = d.id", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var emp = new Employee()
                            {
                                Name = reader.GetString(0),
                                Inn = reader.GetString(1),
                                Department = reader.GetString(2)
                            };
                            employees.Add(emp);
                        }
                    }
                }
            }

            return employees;
        }

        private async Task FillEmployeesSalary(IEnumerable<Employee> employees)
        {
            //todo для улучшения производительности (при n сотрудников 2n запросов - многовато в целом)
            //с учетом тротлинга обоих сервисов можно попытаться отсылать запросы асинхронно пачками, а не последовательно по одному
            //или же добавлять новую версию контрактов на этих сервисах
            foreach (var employee in employees)
            {
                var buhCode = await this.buhService.GetEmployeeBuhCode(employee.Inn);
                employee.Salary = await this.salaryService.GetEmployeeSalaryByInnBuh(employee.Inn, employee.EmployeeBuhCode);
            }
        }
    }
}