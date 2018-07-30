using Gigya.Module.Connector.Logging;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.DeleteSync;
using Gigya.Module.DeleteSync.Providers;
using Gigya.Sitefinity.Module.DeleteSync.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Scheduling;

namespace Gigya.Sitefinity.Module.DeleteSync.Tasks
{
    public class DeleteSyncTask : ScheduledTask
    {
        private const string _key = "bfc466d8-d347-4803-b4e0-f5e25a7418e1";
        private readonly SettingsHelper _settingsHelper = new SettingsHelper();
        private readonly Logger _logger = new Logger(new SitefinityLogger());

        public DeleteSyncTask()
        {
            this.Key = _key;
        }

        public override void ExecuteTask()
        {
            // get settings
            var settings = _settingsHelper.GetSettings();
            if (settings == null)
            {
                _logger.Debug("No delete sync settings so aborting task.");
                return;
            }

            if (!settings.Enabled)
            {
                _logger.Debug("Delete sync module not enabled so aborting task.");
                return;
            }

            var provider = new AmazonProvider(settings.S3AccessKey, settings.S3SecretKey, settings.S3BucketName, settings.S3ObjectKeyPrefix, _logger);
            var service = new DeleteSyncService(provider);
            var uids = service.GetUids().Result;
            if (uids == null || !uids.Any())
            {
                _logger.Debug("No UID's to delete.");
                return;
            }

            // process the files
            var helper = new DeleteSyncHelper();
            helper.Process(settings, uids);

            // schedule again
            var nextProcessDate = DateTime.Now.AddMinutes(settings.FrequencyMins);
            ScheduleTask(nextProcessDate);
        }

        public static void ScheduleTask(DateTime executeTime)
        {
            SchedulingManager schedulingManager = SchedulingManager.GetManager();

            var existingTask = schedulingManager.GetTaskData().FirstOrDefault(x => x.Key == _key);

            if (existingTask == null)
            {
                // Create a new scheduled task
                DeleteSyncTask newTask = new DeleteSyncTask()
                {
                    ExecuteTime = executeTime
                };

                schedulingManager.AddTask(newTask);
            }
            else
            {
                // Updates the existing scheduled task
                existingTask.ExecuteTime = executeTime;
            }

            SchedulingManager.RescheduleNextRun();
            schedulingManager.SaveChanges();
        }

        public override string TaskName
        {
            get
            {
                return this.GetType().FullName;
            }
        }
    }
}
