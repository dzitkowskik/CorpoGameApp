using System.Collections.Generic;
using System.Threading.Tasks;

namespace CorpoGameApp.Services
{
    public interface IEmailServices
    {
        Task SendEmail(string subject, string body, IEnumerable<string> recipients);
    }
}