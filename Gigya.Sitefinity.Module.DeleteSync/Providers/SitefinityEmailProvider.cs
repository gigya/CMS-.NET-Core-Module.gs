using Gigya.Module.DeleteSync.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web.Mail;

namespace Gigya.Sitefinity.Module.DeleteSync.Providers
{
    public class SitefinityEmailProvider : IEmailProvider
    {
        protected readonly EmailSender _sender = EmailSender.Get();

        public void Send(MailMessage message)
        {
            if (message.From == null)
            {
                var smtpSettings = Config.Get<SystemConfig>().SmtpSettings;
                message.From = new MailAddress(smtpSettings.DefaultSenderEmailAddress);
            }
            _sender.Send(message);
        }
    }
}
