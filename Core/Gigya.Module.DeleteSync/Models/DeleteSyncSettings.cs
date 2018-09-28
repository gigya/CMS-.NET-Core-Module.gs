using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.DeleteSync.Models
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
        public string S3Region { get; set; }
        public int MaxAttempts { get; set; } = 1;
    }

    public enum DeleteSyncAction
    {
        DeleteNotification,
        FullUserDeletion
    }
}
