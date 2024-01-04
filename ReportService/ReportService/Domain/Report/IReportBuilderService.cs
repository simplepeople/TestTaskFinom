using System.Threading.Tasks;

namespace ReportService.Domain.Report
{
    public interface IReportBuilderService
    {
        Task<string> CreateReport(int year, int month);
    }
}