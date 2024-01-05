using System.Diagnostics;
using ReportService.Domain.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using NSubstitute;
using ReportService.Domain.Buh;
using ReportService.Domain.Empl;
using ReportService.Domain.Report.Models;
using Xunit;
using ReportService.Domain.Empl.Reader;
using ReportService.Domain.Empl.Reader.Models;
using ReportService.Domain.Salary;

namespace ReportServiceTests
{
    public class EmployeeServiceTests
    {
        private readonly EmployeeDb dbEmployee;

        private readonly Employee employee;

        public EmployeeServiceTests()
        {
            this.dbEmployee = new Faker<EmployeeDb>()
                     .RuleFor(e => e.Name, f => f.Name.FullName())
                     .RuleFor(e => e.Department, f => f.Commerce.Department())
                     .RuleFor(e => e.Inn, f => f.Finance.Account(13))
                     .Generate(1).First();
            this.employee = new Faker<Employee>()
                .RuleFor(e => e.Salary, f => (int)f.Finance.Amount(10000, 150000))
                .Generate(1).First();
            this.employee.Name = this.dbEmployee.Name;
        }

        [Fact]
        public async Task GetAll_EmployeesDepartmentsExisted_EmployeesReturn()
        {
            // Arrange
            var employeeReaderFake = Substitute.For<IEmployeeReader>();
            var dbEmployees = new List<EmployeeDb> { this.dbEmployee };
            employeeReaderFake.ReadEmployees().Returns(new List<EmployeeDb> { this.dbEmployee });

            var buhCode = new Faker().Finance.Account(10);
            var buhServiceFake = Substitute.For<IBuhService>();
            buhServiceFake.GetEmployeeBuhCode(dbEmployee.Inn).Returns(Task.FromResult(buhCode));

            var salaryServiceFake = Substitute.For<ISalaryService>();
            salaryServiceFake.GetEmployeeSalary(this.dbEmployee.Inn, buhCode).Returns(Task.FromResult(this.employee.Salary));

            var employeeService = new EmployeeService(employeeReaderFake, buhServiceFake, salaryServiceFake);

            // Act
            var employeesActual = await employeeService.GetAll();

            // Assert
            employeesActual.Should().NotBeNull().And.HaveCount(1);

            var employeeActual = employeesActual.First();
            employeeActual.Department.Should().Be(this.dbEmployee.Department);
            employeeActual.Name.Should().Be(this.dbEmployee.Name);
            employeeActual.Salary.Should().Be(this.employee.Salary);

            await employeeReaderFake.Received(1).ReadEmployees();
            await buhServiceFake.Received(1).GetEmployeeBuhCode(this.dbEmployee.Inn);
            await salaryServiceFake.Received(1).GetEmployeeSalary(dbEmployee.Inn, buhCode);
        }
    }
}
