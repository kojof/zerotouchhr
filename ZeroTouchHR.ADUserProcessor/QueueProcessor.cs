using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using ZeroTouchHR.Services;
using ZeroTouchHR.Services.Interfaces;

namespace ZeroTouchHR.ADUserProcessor
{
    public class QueueProcessor: IQueueProcessor
    {
        private readonly ISQSService _sQSService;
        private readonly IConfiguration _configuration;
        public QueueProcessor(ISQSService sQSService, IConfiguration configuration)
        {
            _sQSService = sQSService;
            _configuration = configuration;
        }


        public async Task<IEnumerable<string>> ReceiveMessageAsync(string queueName)
        {
            var messages = await _sQSService.ReceiveMessageAsync(queueName);
            return messages.Select(x => x.Body);
        }

        public  void SaveMessageToBatchFile(IEnumerable<string> messageList)
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

                 RunBatchFile(fileDirectory, fileName);
            }
        }

        private void RunBatchFile(string fileDirectory, string fileName)
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
