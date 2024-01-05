using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReportService.Domain.Report;

namespace ReportService.Controllers
{
    [Route("api/[controller]")]
    public class ReportController : Controller
    {
        private readonly IReportBuilderService reportBuilderService;

        public ReportController(IReportBuilderService reportBuilderService)
        {
            this.reportBuilderService = reportBuilderService;
        }

        [HttpGet]
        [Route("{year}/{month}")]
        public async Task<IActionResult> Download(int year, int month)
        {
            //todo хорошо бы добавить валидацию на год и месяц, хотя в текущей реализации оно влияет только на название отчета

            //todo в целом подход с синхронной генерацией тяжелого файла и отдачей его пользователю лучше заменить на отправку отчета, например на почту,
            //чтобы пользователю не было нужды все время ожидать в браузере (и временами не дожидаться результата из-за таймаута)
            //если будет потребность дергать отчет с API, то можно реализовать через вебхук
            //плюс есть вопросы по безопасности сервиса - в текущем виде ок, только при условии, если все находится в интранете

            var reportText = await this.reportBuilderService.CreateReport(year, month);
            
            var file = Encoding.UTF8.GetBytes(reportText);

            //todo обработка ошибок?
            const string responseFileName = "report.txt";
            return File(file, MediaTypeNames.Application.Octet, responseFileName);
        }
    }
}
