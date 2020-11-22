using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZeroTouchHR.Services;
using ZeroTouchHR.Services.Interfaces;

namespace ZeroTouchHR.ADUserProcessor
{
    class Program
    {
        static async Task  Main(string[] args)
        { 
            var startup = new Startup();

            var service = startup.Provider.GetRequiredService<IQueueProcessor>();
          

            //.("BatchFile").GetSection("FilePath").Value;
            string queueName = "UsersForActiveDirectorSQSQueue";
            var messageList = await service.ReceiveMessageAsync(queueName);

            service.SaveMessageToBatchFile(messageList);

            Console.WriteLine("Get Messages!!");
        }

       
    }
}
