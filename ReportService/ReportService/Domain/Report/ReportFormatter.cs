using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReportService.Domain.Report.Models;

namespace ReportService.Domain.Report
{
    //todo решение на базе StringBuilder надежное и простое, особенно если отчет небольшой,
    //но с более сложными макетами лучше использовать что-то по типу Razor'а
    //интерфейс добавлен сразу т.к. часто требуется иметь несколько версий отчетов - txt/html/csv
    internal sealed class ReportPlainTextFormatter : IReportFormatter
    {
        private const string SectionSeparator = "--------------------------------------------";
        private const string SpaceAfterEmployeeName = "         ";
        private const string SpaceAfterTotal = "         ";
        private const string DepartmentTotal = "Всего по отделу";
        private const string CompanyTotal = "Всего по предприятию";
        private const string SalaryCurrencySymbol = "р";

        private readonly StringBuilder sbContent = new StringBuilder();

        //todo если же нужно чуть менять отчет в зависимости от данных
        //шаблонный метод наш лучший друг
        public string CreateReport(ReportData data)
        {
            AppendDateHeader(data);
            AppendDepartments(data.Company.Departments);
            AppendTotalSalary(data.Company.TotalSalary);

            return this.sbContent.ToString();
        }

        private void AppendDateHeader(ReportData data)
        {
            sbContent.AppendLine(new DateTime(data.Year, data.Month, 1).ToString("MMMM yyyy", new CultureInfo("Ru-ru")));
            AppendSectionSeparator();
        }

        private void AppendDepartments(IEnumerable<Department> departments)
        {
            foreach (var department in departments)
            {
                AppendDepartment(department);
            }
        }

        private void AppendDepartment(Department department)
        {
            this.sbContent.AppendLine(department.Name);
            AppendEmployees(department.Employees);
            AppendDepartmentTotalSalary(department.TotalSalary);
        }

        private void AppendEmployees(IEnumerable<Employee> departmentEmployees)
        {
            foreach (var employee in departmentEmployees)
            {
                AppendEmployee(employee);
            }
        }

        private void AppendEmployee(Employee employee)
        {
            this.sbContent.Append(employee.Name);
            this.sbContent.Append(SpaceAfterEmployeeName);
            AppendSalary(employee.Salary);
            this.sbContent.AppendLine();
        }

        private void AppendDepartmentTotalSalary(int departmentTotalSalary)
        {
            this.sbContent.Append(DepartmentTotal);
            this.sbContent.Append(SpaceAfterTotal);
            this.AppendSalary(departmentTotalSalary);
            this.sbContent.AppendLine();
            this.AppendSectionSeparator();
        }

        private void AppendTotalSalary(int companyTotalSalary)
        {
            this.sbContent.Append(CompanyTotal);
            this.sbContent.Append(SpaceAfterTotal);
            this.AppendSalary(companyTotalSalary);
        }

        private void AppendSalary(int salary)
        {
            this.sbContent.Append(salary);
            this.sbContent.Append(SalaryCurrencySymbol);
        }

        private void AppendSectionSeparator()
        {
            this.sbContent.AppendLine(SectionSeparator);
        }
    }
}
