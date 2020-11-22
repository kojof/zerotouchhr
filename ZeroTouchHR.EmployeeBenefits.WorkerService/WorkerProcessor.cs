using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ZeroTouchHR.Services.Interfaces;

namespace ZeroTouchHR.EmployeeBenefits.WorkerService
{
    public class WorkerProcessor : BackgroundService
    {
        private readonly ILogger<WorkerProcessor> _logger;
        private readonly IConfiguration _configuration;
        private readonly ISQSService _sQSService;
        private readonly ISESService _sESService;
        public WorkerProcessor(ILogger<WorkerProcessor> logger, ISQSService sQSService, ISESService sESService, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _sQSService = sQSService;
            _sESService = sESService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    //get messages
                    string queueName = "WelcomePackSQSQueue";
                    var result = await _sQSService.ReceiveMessageAsync(queueName);

                    if (result != null && result.Any())
                    {

                        var messages = result.Select(x => x.Body);
                        var userDetails = messages.ToList();
                        var emailAddress = userDetails[0];
                        await _sESService.Send(emailAddress);
                    }

                }
                catch (Exception e)
                {
                    _logger.LogError(e.InnerException.ToString());
                    Console.WriteLine(e.StackTrace);
                    Console.WriteLine(e.InnerException);
                    Console.WriteLine(e);
                 
                    //     throw;
                }

                _logger.LogInformation("WorkerProcessor running at: {time}", DateTimeOffset.Now);


                //   await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
