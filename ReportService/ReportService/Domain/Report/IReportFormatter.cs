using ReportService.Domain.Report.Models;

namespace ReportService.Domain.Report
{
    public interface IReportFormatter
    {
        string CreateReport(ReportData data);
    }
}