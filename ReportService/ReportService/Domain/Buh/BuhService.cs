using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ReportService.Domain.Buh
{
    internal sealed class BuhService : IBuhService
    {
        private readonly Uri baseUri;
        private readonly HttpClient client;
        public BuhService(Uri baseUri)
        {
            this.baseUri = baseUri;
            // todo Заменить на IHttpClientFactory после обновления на Core 3.1 или выше
            this.client = HttpClientFactory.Create();
        }

        //todo анемичная модель?
        public async Task<string> GetEmployeeBuhCode(string inn)
        {
            //todo кеширование, но без контекста непонятно время валидности buh-кода
            return await client.GetStringAsync(new Uri(this.baseUri, $"{inn}/"));
        }
    }
}