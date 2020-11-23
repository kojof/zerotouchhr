using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ZeroTouchHR.Domain.Entities;
using ZeroTouchHR.Services.Interfaces;


namespace ZeroTouchHR.Services
{
    public class SQSService : ISQSService
    {

        private readonly IAmazonSQS _sqs;

        //  private readonly string _sqsUrl;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SQSService> _logger;

        public SQSService(ILogger<SQSService> logger, IAmazonSQS sqs, IConfiguration configuration)
        {
            this._sqs = sqs;
            _configuration = configuration;
            _logger = logger;
        }


        public async Task<bool> SendMessageAsync(ADUserCredentials aDUserCredentials)
        {
            try
            {
                string sqsUrl = _configuration.GetSection("AWS").GetSection("UsersForActiveDirectorSQSQueue").Value;

          //      string message = JsonConvert.SerializeObject(aDUserCredentials.ToString());
          
                var sendRequest = new SendMessageRequest(sqsUrl, aDUserCredentials.ToString());
                sendRequest.MessageGroupId = "ZeroTouchHR";
                // Post message or payload to queue  
                var sendResult = await _sqs.SendMessageAsync(sendRequest);

                return sendResult.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException.ToString());
                throw ex;
            }
        }

        public async Task<IEnumerable<Message>> ReceiveMessageAsync(string queueName)
        {
            try
            {
                string sqsUrl = _configuration.GetSection("AWS").GetSection(queueName).Value;

                //Create New instance  
                var request = new ReceiveMessageRequest
                {
                    QueueUrl = sqsUrl,
                    MaxNumberOfMessages = 10,
                    WaitTimeSeconds = 5
                };
                //CheckIs there any new message available to process  
                var result = await _sqs.ReceiveMessageAsync(request);

                if (result.HttpStatusCode == HttpStatusCode.OK)
                {
                    if(result.Messages.Count > 0)
                    {
                        await DeleteMessageAfterRead(result.Messages, sqsUrl);
                    }
                }
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                return result.Messages.Any() ? result.Messages : new List<Message>();
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.InnerException.ToString());
                throw ex;
            }

            return null;
        }

        private async Task DeleteMessageAfterRead(IEnumerable<Message> messages, string sqsUrl)
        {
            if (messages != null)
            {
                foreach (var message in messages)
                {
                    var messageReceiptHandle = message.ReceiptHandle;

                    var deleteMessageRequest = new DeleteMessageRequest
                    {
                        QueueUrl = sqsUrl,
                        ReceiptHandle = messageReceiptHandle
                    };

                    await _sqs.DeleteMessageAsync(deleteMessageRequest);
                }
            }
        }
    }
}