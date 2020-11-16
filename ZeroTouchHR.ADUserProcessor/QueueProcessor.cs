using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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


        public async Task<List<string>> ReceiveMessageAsync()
        {
            var messages = await _sQSService.ReceiveMessageAsync();

            List<string> messageList = new List<string>();

            foreach (var message in messages)
            {
                messageList.Add(message.Body);
            }
           
            return messageList;
        }

        public  void SaveMessageToBatchFile(List<string> messageList)
        {
            var configuration = _configuration["BatchFilePath"];

            foreach (var message in messageList)
            {
                Console.WriteLine(message);
                string fileName = Guid.NewGuid().ToString() + ".bat";
              //  string filePath = @"C:\\projects\\great learning\\capstone project\\code\batchfiles";
              var fileDirectory = _configuration["BatchFilePath"];

              var filePath = Path.Combine(fileDirectory, fileName);
                //File.WriteAllText(Path.Combine(filePath, fileName), message);
                using (FileStream fs = File.Create(filePath, 1024))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(message);
                    // Add some information to the file.
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
            }
        }
    }
}
