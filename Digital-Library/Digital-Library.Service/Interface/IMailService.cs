using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Service.Interface
{
    public interface IMailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
