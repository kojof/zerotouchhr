using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ZeroTouchHR.Services.Interfaces;

namespace ZeroTouchHR.SQSProcessor.WorkerService
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
                    string queueName = "UsersForActiveDirectorSQSQueue";
                    var result = await _sQSService.ReceiveMessageAsync(queueName);

                 if (result != null)
                 {

                     var messages = result.Select(x => x.Body);
                     await SaveMessageToBatchFile(messages);
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

        private async Task SaveMessageToBatchFile(IEnumerable<string> messageList)
        {
            var configuration = _configuration["BatchFilePath"];

            foreach (var message in messageList)
            {
                Console.WriteLine(message);
                string fileName = Guid.NewGuid().ToString() + ".bat";

                var fileDirectory = _configuration["BatchFilePath"];

                var filePath = Path.Combine(fileDirectory, fileName);

                using (FileStream fs = File.Create(filePath, 1024))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(message);


                    fs.Write(info, 0, info.Length);
                }

                // Open the stream and read it back.
                using (StreamReader sr = File.OpenText(filePath))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(s);

                    }
                }

                await RunBatchFile(fileDirectory, fileName);
            }
        }

        private async Task RunBatchFile(string fileDirectory, string fileName)
        {
            Process process = new Process();
            try
            {
                string targetDir;
                targetDir = string.Format(fileDirectory);   //directory where the batch script is saved.

                process.StartInfo.UseShellExecute = true;
                process.StartInfo.WorkingDirectory = targetDir;
                process.StartInfo.FileName = fileName;          //this is the batch script that needs to run.
                process.StartInfo.CreateNoWindow = false;
                process.Start();
                process.WaitForExit();
                System.Threading.Thread.Sleep(5000);        //Wait 5 seconds to close window.
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
            }
        }
    }
}
