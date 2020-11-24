using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SimpleEmail;
using Amazon.SQS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ZeroTouchHR.EmployeeBenefits.WorkerService.DataModels;
using ZeroTouchHR.Models;
using ZeroTouchHR.Services;
using ZeroTouchHR.Services.Interfaces;


namespace ZeroTouchHR.EmployeeBenefits.WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: false, reloadOnChange: true)
                        .AddEnvironmentVariables()
                        .Build();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                    optionsBuilder.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection"));//,
                    services.AddScoped<ApplicationDbContext>(s => new ApplicationDbContext(optionsBuilder.Options));


                    // AWS Configuration
                    var options = hostContext.Configuration.GetAWSOptions();
                    Console.WriteLine(options);

                    services.AddLogging(config =>
                    {
                        config.AddAWSProvider(hostContext.Configuration.GetAWSLoggingConfigSection());
                        config.SetMinimumLevel(LogLevel.Debug);
                    });
                 
                    services.AddDefaultAWSOptions(options);
                    services.AddAWSService<IAmazonSQS>();
                    services.AddAWSService<IAmazonSimpleEmailService>();
                    services.AddTransient<ISQSService, SQSService>();
                    services.AddTransient<ISESService, SESService>();
                 //   services.AddScoped<IRDSService, RDSService>();

                    // WorkerProcessor Service
                    services.AddHostedService<WorkerProcessor>();
                });
    }
}
