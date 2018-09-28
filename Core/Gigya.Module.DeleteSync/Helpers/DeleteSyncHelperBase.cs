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
            var builder = new StringBuilder();
            builder.AppendLine($"The user deletion job scheduled to run for {_emailModel.Domain} at {_emailModel.DateStarted} UTC, completed with {_emailModel.FailedDeletedUids.Count + _emailModel.FailedUpdatedUids.Count} errors.");

            builder.AppendLine();

            var total = _emailModel.DeletedUids.Count + _emailModel.FailedDeletedUids.Count + _emailModel.FailedUpdatedUids.Count + _emailModel.UpdatedUids.Count;
            builder.AppendLine($"A total of {_emailModel.DeletedUids.Count} out of {total} users were deleted.");
            builder.AppendLine($"A total of {_emailModel.UpdatedUids.Count} out of {total} users were marked for deletion.");

            _logger.Debug(builder.ToString());
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
