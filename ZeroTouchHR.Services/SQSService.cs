using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
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

        //   private readonly ServiceConfiguration _settings;

        public SQSService(IAmazonSQS sqs, IConfiguration configuration)
        {
            this._sqs = sqs;
            //  this._sqsUrl = sqsUrl;
            _configuration = configuration;
        }


        public async Task<bool> SendMessageAsync(ADUserCredentials aDUserCredentials)
        {
            try
            {
                string sqlUrl = _configuration.GetSection("AWS").GetSection("SQS").Value;

                string message = JsonConvert.SerializeObject(aDUserCredentials.ToString());

                var sendRequest = new SendMessageRequest(sqlUrl, message);
                sendRequest.MessageGroupId = "ZeroTouchHR";
                // Post message or payload to queue  
                var sendResult = await _sqs.SendMessageAsync(sendRequest);

                return sendResult.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Message>> ReceiveMessageAsync()
        {
            try
            {
                string sqlUrl = _configuration.GetSection("AWS").GetSection("SQS").Value;

                //Create New instance  
                var request = new ReceiveMessageRequest
                {
                    QueueUrl = sqlUrl,
                    MaxNumberOfMessages = 10,
                    WaitTimeSeconds = 5
                };
                //CheckIs there any new message available to process  
                var result = await _sqs.ReceiveMessageAsync(request);

            //    await DeleteMessageAfterRead(result.Messages, sqlUrl);

                return result.Messages.Any() ? result.Messages : new List<Message>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task DeleteMessageAfterRead(List<Message> messages, string sqlUrl)
        {
            if (messages != null)
            {
                foreach (var message in messages)
                {

                    var messageReceiptHandle = message.ReceiptHandle;

                    Task<DeleteMessageResponse> DeleteMessageResponse;
                    DeleteMessageRequest deleteMessageRequest = new DeleteMessageRequest()
                    {
                        QueueUrl = sqlUrl,
                        ReceiptHandle = messageReceiptHandle
                    };

                    DeleteMessageResponse = _sqs.DeleteMessageAsync(deleteMessageRequest);
                }

            }
        }
    }
}