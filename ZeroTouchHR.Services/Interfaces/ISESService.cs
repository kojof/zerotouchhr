using System.Net;
using System.Threading.Tasks;

namespace ZeroTouchHR.Services.Interfaces
{
    public interface ISESService
    {
        Task<HttpStatusCode> SendEmailAsync(string emailAddress);
    }
}
