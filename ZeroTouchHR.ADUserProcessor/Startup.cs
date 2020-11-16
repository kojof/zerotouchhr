using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZeroTouchHR.Services;
using ZeroTouchHR.Services.Interfaces;

namespace ZeroTouchHR.ADUserProcessor
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _provider;

        // access the built service pipeline
        public IServiceProvider Provider => _provider;

        // access the built configuration
        public IConfiguration Configuration => _configuration;

        
        public Startup()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            // instantiate
            var services = new ServiceCollection();

            // add necessary services
            services.AddSingleton<IConfiguration>(_configuration);
            services.AddTransient<ISQSService, SQSService>();
            services.AddTransient<IQueueProcessor, QueueProcessor>();
            services.AddAWSService<IAmazonSQS>();

            // build the pipeline
            _provider = services.BuildServiceProvider();
        }
    }
}
