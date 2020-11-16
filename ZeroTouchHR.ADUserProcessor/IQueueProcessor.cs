using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon.SQS.Model;

namespace ZeroTouchHR.ADUserProcessor
{
    public interface IQueueProcessor
    {
        Task<List<string>> ReceiveMessageAsync();
        void SaveMessageToBatchFile(List<string> messageList);
    }
}
