using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon.SQS.Model;

namespace ZeroTouchHR.ADUserProcessor
{
    public interface IQueueProcessor
    {
        Task<IEnumerable<string>> ReceiveMessageAsync(string queueName);
        void SaveMessageToBatchFile(IEnumerable<string> messageList);
    }
}
