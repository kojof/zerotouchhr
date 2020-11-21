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
        private readonly IAmazonSQS _sqs;
        private readonly IConfiguration _configuration;
        private readonly ISQSService _sQSService;

        public WorkerProcessor(ILogger<WorkerProcessor> logger, ISQSService sQSService, IConfiguration configuration)
        {
            _logger = logger;
            //   _sqs = sqs;
            _configuration = configuration;
            _sQSService = sQSService;
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

                    if (result != null)
                    {

                        var messages = result.Select(x => x.Body);
                       // await SaveMessageToBatchFile(messages);
                    }

                }
                catch (Exception e)
                {
                    _logger.LogError(e.InnerException.ToString());
                    Console.WriteLine(e);
                    throw;
                }

                _logger.LogInformation("WorkerProcessor running at: {time}", DateTimeOffset.Now);


                //   await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
