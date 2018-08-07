using Gigya.Module.DeleteSync.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.DeleteSync.Tests.Providers
{
    public class FakeEmailProvider : IEmailProvider
    {
        public void Send(MailMessage message)
        {
            var smtpClient = new SmtpClient();
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            smtpClient.PickupDirectoryLocation = "C:\\temp\\gigya";
            smtpClient.Send(message);
        }
    }
}
