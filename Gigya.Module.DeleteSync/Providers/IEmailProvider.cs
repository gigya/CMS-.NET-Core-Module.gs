using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.DeleteSync.Providers
{
    public interface IEmailProvider
    {
        void Send(MailMessage message);
    }
}
