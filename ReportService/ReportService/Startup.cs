using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReportService.Domain.Buh;
using ReportService.Domain.Empl;
using ReportService.Domain.Empl.Reader;
using ReportService.Domain.Report;
using ReportService.Domain.Salary;

namespace ReportService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //todo подцепить swagger и хелсчеки?
            //в идеале сначала обновить до последнего .NET, но только после деплоя этой версии,
            //чтобы не ловить инфраструктурные проблемы

            services.AddMvc();

            //var configuration = services.BuildServiceProvider().GetService<IConfiguration>();

            services.AddTransient<IReportBuilderService, ReportBuilderService>();
            services.AddTransient<IEmployeeService, EmployeeService>();
            services.AddTransient<IReportFormatter, ReportPlainTextFormatter>();
            services.AddSingleton<ISalaryService>(provider => new SalaryService(new Uri(Configuration.GetSection("AppSettings")["SalaryServiceUrl"])));
            services.AddSingleton<IBuhService>(provider => new BuhService(new Uri(Configuration.GetSection("AppSettings")["BuhServiceUrl"])));
            services.AddTransient<IEmployeeReader>(provider => new EmployeeDbReader(Configuration.GetConnectionString("EmployeeConnectionString")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
