using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ReportService.Domain.Salary
{
    internal sealed class SalaryService : ISalaryService
    {
        private readonly Uri baseUri;
        private readonly HttpClient client;
        public SalaryService(Uri baseUri)
        {
            this.baseUri = baseUri;

            // todo Заменить на IHttpClientFactory после обновления на Core 3.1 или выше
            this.client = HttpClientFactory.Create();
        }

        public async Task<int> GetEmployeeSalary(string inn, string employeeBuhCode)
        {
            //todo можно прикрутить Polly
            //todo из-за старого кора за неимением возможности заинжектить фабрику, используем вот это
            var resp = await client.PostAsJsonAsync(new Uri(this.baseUri, $"{inn}/"), new { employeeBuhCode });
            resp.EnsureSuccessStatusCode();
            var content = await resp.Content.ReadAsStringAsync();
            var salary = (int)decimal.Parse(content);
            return salary;
        }
    }
}