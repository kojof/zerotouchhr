using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ZeroTouchHR.EmployeeBenefits.WorkerService.DataModels;
using ZeroTouchHR.Models;
using ZeroTouchHR.Services;
using ZeroTouchHR.Services.Interfaces;


namespace ZeroTouchHR.EmployeeBenefits.WorkerService
{
    public class WorkerProcessor : BackgroundService
    {
        private readonly ILogger<WorkerProcessor> _logger;
        private readonly IConfiguration _configuration;
        private readonly ISQSService _sQSService;
        private readonly ISESService _sESService;

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public WorkerProcessor(ILogger<WorkerProcessor> logger, ISQSService sQSService, ISESService sESService,  IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _sQSService = sQSService;
            _sESService = sESService;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Entered WorkerProcessor running at: {time}", DateTimeOffset.Now);

                    string queueName = "WelcomePackSQSQueue";
                  //  string queueName = "HealthBenefitsSQSQueue";

                    var response = await _sQSService.ReceiveMessageAsync(queueName);

                    _logger.LogInformation("WorkerProcessor Get Responses at: {time} {response} ", DateTimeOffset.Now, response);

                    if (response != null && response.Any())
                    {

                        var snsMessages = response.Select(x => x.Body).ToList();

                        _logger.LogInformation("WorkerProcessor Response received at: {time} {snsMessages}", DateTimeOffset.Now, snsMessages);

                        foreach (var message in snsMessages)
                        {
                            var snsMessage = Amazon.SimpleNotificationService.Util.Message.ParseMessage(message);

                            var result = snsMessage.MessageText;
                            string replacedCharacter = result.Replace("%40", "@");
                            var separator = "/";

                            int separatorIndex = replacedCharacter.IndexOf(separator);
                            string emailAddress = replacedCharacter.Substring(0, separatorIndex);

                            _logger.LogInformation("WorkerProcessor Send message containing EmailAddress to SES at: {time} {emailAddress}", DateTimeOffset.Now, emailAddress);
                            
                            var emailResponse = await _sESService.SendEmailAsync(emailAddress);

                            if (emailResponse == HttpStatusCode.OK)
                            {
                                using var scope = _serviceScopeFactory.CreateScope();

                                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                                _logger.LogInformation("WorkerProcessor start to update database at: {time}", DateTimeOffset.Now);

                                //Update Database
                                var employee = await dbContext.employee.Where(x => x.Email == emailAddress).FirstOrDefaultAsync();
                                employee.Status = "Completed";
                                await dbContext.SaveChangesAsync();

                                _logger.LogInformation("WorkerProcessor updated database at: {time}", DateTimeOffset.Now);
                            }
                            
                            _logger.LogInformation("WorkerProcessor Sent message containing EmailAddress to SES at: {time} {emailAddress}", DateTimeOffset.Now, emailAddress);
                        }
                    }

                }
                catch (Exception e)
                {
                    _logger.LogError("WorkerProcessor Sent message Exception {exception}", e.InnerException.ToString());
                    _logger.LogError("WorkerProcessor Sent message Exception {exception}", e.StackTrace.ToString());
                }

                _logger.LogInformation("WorkerProcessor running at: {time}", DateTimeOffset.Now);


                //   await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
