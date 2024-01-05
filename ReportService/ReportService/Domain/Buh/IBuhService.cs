using System.Threading.Tasks;

namespace ReportService.Domain.Buh
{
    internal interface IBuhService
    {
        Task<string> GetEmployeeBuhCode(string inn);
    }
}