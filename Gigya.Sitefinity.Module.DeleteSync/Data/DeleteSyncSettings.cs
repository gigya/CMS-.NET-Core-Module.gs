using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Sitefinity.Module.DeleteSync.Data
{
    public class DeleteSyncSettings
    {
        public int Id { get; set; }
        public DeleteSyncAction Action { get; set; }
        public bool Enabled { get; set; }
        public int FrequencyMins { get; set; }
        public string EmailsOnSuccess { get; set; }
        public string EmailsOnFailure { get; set; }
        public string S3BucketName { get; set; }
        public string S3AccessKey { get; set; }
        public string S3SecretKey { get; set; }
        public string S3ObjectKeyPrefix { get; set; }
    }

    public enum DeleteSyncAction
    {
        DeleteNotification,
        FullUserDeletion
    }
}
