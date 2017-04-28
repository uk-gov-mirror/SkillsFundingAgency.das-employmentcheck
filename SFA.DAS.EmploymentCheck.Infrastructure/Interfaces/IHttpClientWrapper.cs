using System.Threading.Tasks;

namespace SFA.DAS.EmploymentCheck.Infrastructure.Interfaces
{
    public interface IHttpClientWrapper
    {
        Task<string> SendMessage<T>(T content, string url);
        Task<T> Get<T>(string authToken, string url);
    }
}
