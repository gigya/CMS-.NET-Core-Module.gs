using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.DeleteSync.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Gigya.Module.DeleteSync.Helpers
{
    public abstract class DeleteSyncHelperBase
    {
        protected readonly Logger _logger;
        protected readonly EmailHelper _emailHelper;
        protected DeleteSyncEmailModel _emailModel;

        public DeleteSyncHelperBase(EmailHelper emailHelper, Logger logger)
        {
            _emailHelper = emailHelper;
            _logger = logger;
        }

        protected void WriteSummaryLog()
        {
            if (string.IsNullOrEmpty(_emailModel.Body))
            {
                SetEmailBody();
            }

            _logger.Debug(_emailModel.Body);
        }

        protected void SetEmailBody()
        {
            var model = _emailModel;
            var builder = new StringBuilder();
            builder.AppendLine($"The user deletion job scheduled to run for {model.Domain} at {model.DateStarted} UTC, completed with {model.FailedDeletedUids.Count + model.FailedUpdatedUids.Count} errors.");

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

            builder.AppendLine("Accounts marked for deletion:");
            builder.AppendLine($"[{string.Join(", ", model.UpdatedUids.Select(i => $"\"{i}\""))}]");
            builder.AppendLine();

            builder.AppendLine("Accounts deleted:");
            builder.AppendLine($"[{string.Join(", ", model.DeletedUids.Select(i => $"\"{i}\""))}]");
            builder.AppendLine();

            builder.AppendLine("Accounts failed to be marked for deletion:");
            builder.AppendLine($"[{string.Join(", ", model.FailedUpdatedUids.Select(i => $"\"{i}\""))}]");
            builder.AppendLine();

            builder.AppendLine("Accounts failed to be deleted:");
            builder.AppendLine($"[{string.Join(", ", model.FailedDeletedUids.Select(i => $"\"{i}\""))}]");
            builder.AppendLine();

            _emailModel.Body = builder.ToString();
        }

        protected void AddLogEntry(bool success, string uid, DeleteSyncAction action)
        {
            switch (action)
            {
                case DeleteSyncAction.FullUserDeletion:
                    if (success)
                    {
                        _emailModel.DeletedUids.Add(uid);
                    }
                    else
                    {
                        _emailModel.FailedDeletedUids.Add(uid);
                    }
                    return;
                case DeleteSyncAction.DeleteNotification:
                    if (success)
                    {
                        _emailModel.UpdatedUids.Add(uid);
                    }
                    else
                    {
                        _emailModel.FailedUpdatedUids.Add(uid);
                    }
                    return;
                default:
                    _logger.Error($"Action: {action} not supported.");
                    return;
            }
        }
    }
}
