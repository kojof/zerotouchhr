using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SQS.Model;
using ZeroTouchHR.Domain.Entities;

namespace ZeroTouchHR.Services.Interfaces
{
    public interface ISQSService
    {
        Task<bool> SendMessageAsync(ADUserCredentials aDUserCredentials);
        Task<List<Message>> ReceiveMessageAsync();
    }
}