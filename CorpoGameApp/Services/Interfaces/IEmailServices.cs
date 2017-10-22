using System.Collections.Generic;
using System.Threading.Tasks;
using SendGrid;

namespace CorpoGameApp.Services
{
    public interface IEmailServices
    {
        Task<Response> SendEmail(string subject, string body, IEnumerable<string> recipients);
    }
}