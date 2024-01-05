using System.Diagnostics;
using ReportService.Domain.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using NSubstitute;
using ReportService.Domain.Empl;
using ReportService.Domain.Report.Models;
using Xunit;
using Employee = ReportService.Domain.Empl.Models.Employee;

namespace ReportServiceTests
{
    public class ReportBuilderServiceTests
    {
        private readonly List<Employee> employees;

        public ReportBuilderServiceTests()
        {
            this.employees = new Faker<Employee>()
                .RuleFor(e => e.Name, f => f.Name.FullName())
                .RuleFor(e => e.Salary, f => (int)f.Finance.Amount(10000, 150000))
                .RuleFor(e => e.Department, f => f.Commerce.Department(4))
                .Generate(3);
        }

        [Theory]
        [InlineData(2014, 1)]
        public async Task CreateReport_EmployeesDepartmentsExisted_ReportCreated(int year, int month)
        {
            // Arrange
            var employeeServiceFake = Substitute.For<IEmployeeService>();
            employeeServiceFake.GetAll().Returns(this.employees);
            string reportExpectedContent = new Faker().Lorem.Text();
            var reportFormatterFake = Substitute.For<IReportFormatter>();
            reportFormatterFake.CreateReport(Arg.Any<ReportData>()).Returns(reportExpectedContent);
            var reportBuilderService = new ReportBuilderService(employeeServiceFake, reportFormatterFake);

            // Act
            var reportActualContent = await reportBuilderService.CreateReport(year, month);

            // Assert
            reportActualContent.Should().Be(reportExpectedContent);

            reportFormatterFake.Received(1).CreateReport(Arg.Is<ReportData>(rd =>
                rd.Year == year
                && rd.Month == month
                && rd.Company != null
                && rd.Company.TotalSalary == this.employees.Sum(x => x.Salary)
                && rd.Company.Departments != null
                && rd.Company.Departments.Select(d=>d.Name).OrderBy(n=>n).SequenceEqual(this.employees.Select(e => e.Department).OrderBy(n=>n).Distinct())

                //todo add employee checks
            ));
        }
    }
}
