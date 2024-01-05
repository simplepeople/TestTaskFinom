using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;
using ReportService.Domain.Empl.Models;
using ReportService.Domain.Empl.Reader.Models;

namespace ReportService.Domain.Empl.Reader
{
    internal sealed class EmployeeDbReader : IEmployeeReader
    {
        private readonly string connectionString;

        public EmployeeDbReader(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<List<EmployeeDb>> ReadEmployees()
        {
            var employees = new List<EmployeeDb>();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                //todo мне сложно представить, что в таблице emps данные о сотрудниках могут дублироваться
                //todo также сложно представить, что FK на deps может быть невалидным
                //в противном случае лучше предварительно восстановить консистентность записей в БД 
                //поэтому не вижу здесь смысла делать LEFT join, также как и отдельный запрос в deps
                using (var command = new NpgsqlCommand("SELECT e.name, e.inn, d.name from emps e join deps d on e.departmentid = d.id", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var emp = new EmployeeDb()
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
    }
}