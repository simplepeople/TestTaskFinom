using System.Net.Http;
using System.Threading.Tasks;

namespace ReportService.Domain.Buh
{
    public class BuhService
    {
        //todo анемичная модель? Только не в рамках тестового
        public async Task<string> GetEmployeeBuhCode(string inn)
        {
            //todo кеширование, но без контекста непонятно время валидности buh-кода
            var client = HttpClientFactory.Create();
            return await client.GetStringAsync($"http://buh.local/api/inn/{inn}");
        }
    }
}