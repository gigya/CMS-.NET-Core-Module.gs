using Gigya.Module.DeleteSync.Models;
using Gigya.Module.DeleteSync.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.DeleteSync.Helpers
{
    public class EmailHelper
    {
        protected readonly IEmailProvider _emailProvider;

        public EmailHelper(IEmailProvider emailProvider)
        {
            _emailProvider = emailProvider;
        }

        public void SendConfirmation(DeleteSyncEmailModel model)
        {
            var message = new MailMessage();
            message.To.Add(model.To);
            if (!string.IsNullOrEmpty(model.From))
            {
                message.From = new MailAddress(model.From);
            }
            message.Subject = model.Subject;
            message.Body = model.Body;

            _emailProvider.Send(message);
        }
    }
}
