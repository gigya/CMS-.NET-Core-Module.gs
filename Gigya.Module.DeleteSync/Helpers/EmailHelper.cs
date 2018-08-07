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
            message.Body = GetEmailBody(model);

            _emailProvider.Send(message);
        }

        protected string GetEmailBody(DeleteSyncEmailModel model)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"The user deletion job scheduled to run for {model.Domain} at {model.DateStarted}, completed with {model.FailedDeletedUids.Count + model.FailedUpdatedUids.Count} errors.");

            builder.AppendLine();

            var total = model.DeletedUids.Count + model.FailedDeletedUids.Count + model.FailedUpdatedUids.Count + model.UpdatedUids.Count;
            builder.AppendLine($"A total of {model.DeletedUids.Count} out of {total} users were deleted.");
            builder.AppendLine($"A total of {model.UpdatedUids.Count} out of {total} users were marked for deletion.");

            builder.AppendLine();
            builder.AppendLine("===================================================");
            builder.AppendLine();
            builder.AppendLine("Detailed information:");
            builder.AppendLine();

            builder.AppendLine("Processed files:");
            foreach (var file in model.ProcessedFilenames)
            {
                builder.AppendLine(file);
            }
            builder.AppendLine();

            builder.AppendLine("Accounts updated:");
            builder.AppendLine($"[{string.Join(", ", model.UpdatedUids.Select(i => $"\"{i}\""))}]");
            builder.AppendLine();

            builder.AppendLine("Accounts deleted:");
            builder.AppendLine($"[{string.Join(", ", model.DeletedUids.Select(i => $"\"{i}\""))}]");
            builder.AppendLine();

            builder.AppendLine("Accounts failed to be updated:");
            builder.AppendLine($"[{string.Join(", ", model.FailedUpdatedUids.Select(i => $"\"{i}\""))}]");
            builder.AppendLine();

            builder.AppendLine("Accounts failed to be deleted:");
            builder.AppendLine($"[{string.Join(", ", model.FailedDeletedUids.Select(i => $"\"{i}\""))}]");
            builder.AppendLine();
            return builder.ToString();
        }
    }
}
