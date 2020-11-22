using System.Threading.Tasks;

namespace ZeroTouchHR.Services.Interfaces
{
    public interface ISESService
    {
        Task Send(string emailAddress);
    }
}
