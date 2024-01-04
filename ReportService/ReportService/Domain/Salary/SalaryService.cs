using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ReportService.Domain.Salary
{
    public class SalaryService
    {
        public async Task<int> GetEmployeeSalaryByInnBuh(string inn, string employeeBuhCode)
        {
            //todo использовать refit или аналогичную либо будет удобнее
            //в рамках тестового оставляю почти как есть, лишь меня на async
            //и добавляю вызов Dispose
            var httpWebRequest = (HttpWebRequest)WebRequest.Create($"http://salary.local/api/empcode/{inn}");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = WebRequestMethods.Http.Post;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(new { employeeBuhCode });
                await streamWriter.WriteAsync(json);
                await streamWriter.FlushAsync();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string responseText;
            using (var reader = new StreamReader(httpResponse.GetResponseStream(), true))
            {
                responseText = await reader.ReadToEndAsync();
            }

            return (int)decimal.Parse(responseText);
        }
    }
}