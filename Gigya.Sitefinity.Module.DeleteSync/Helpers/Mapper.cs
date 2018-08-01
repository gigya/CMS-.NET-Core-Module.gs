using Gigya.Module.DeleteSync.Models;
using Gigya.Sitefinity.Module.DeleteSync.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Sitefinity.Module.DeleteSync.Helpers
{
    public static class Mapper
    {
        public static DeleteSyncLog Map(SitefinityDeleteSyncLog source)
        {
            return new DeleteSyncLog
            {
                DateCreated = source.DateCreated,
                Errors = source.Errors,
                Key = source.Key,
                Success = source.Success,
                Total = source.Total
            };
        }

        public static DeleteSyncSettings Map(SitefinityDeleteSyncSettings source)
        {
            return new DeleteSyncSettings
            {
                Action = source.Action,
                EmailsOnFailure = source.EmailsOnFailure,
                EmailsOnSuccess = source.EmailsOnSuccess,
                Enabled = source.Enabled,
                FrequencyMins = source.FrequencyMins,
                Id = source.Id,
                S3AccessKey = source.S3AccessKey,
                S3BucketName = source.S3BucketName,
                S3ObjectKeyPrefix = source.S3ObjectKeyPrefix,
                S3Region = source.S3Region,
                S3SecretKey = source.S3SecretKey,
                MaxAttempts = source.MaxAttempts
            };
        }
    }
}
