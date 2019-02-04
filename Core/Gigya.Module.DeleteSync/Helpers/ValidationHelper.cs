using Gigya.Module.DeleteSync;
using Gigya.Module.DeleteSync.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gigya.Module.DeleteSync.Helpers
{
    public class ValidationHelper : ValidationHelper<DeleteSyncSettings>
    {
        public ValidationHelper(IDeleteSyncService deleteSyncService) : base(deleteSyncService)
        {
        }
    }

    public class ValidationHelper<T> where T : DeleteSyncSettings
    {
        protected readonly IDeleteSyncService _deleteSyncService;

        public ValidationHelper(IDeleteSyncService deleteSyncService)
        {
            _deleteSyncService = deleteSyncService;
        }

        /// <summary>
        /// Validates the settings and returns an error message if invalid.
        /// </summary>
        /// <param name="settings">The settings to validate.</param>
        /// <returns></returns>
        public virtual ValidationResponseModel Validate(T settings)
        {
            var response = new ValidationResponseModel();

            if (!settings.Enabled)
            {
                response.IsValid = true;
                return response;
            }

            if (settings.MaxAttempts < 1)
            {
                response.Message = "Max attempts should be greater than 0.";
                return response;
            }

            if (settings.FrequencyMins <= 0)
            {
                response.Message = "Frequency minutes must be greater than 0.";
                return response;
            }

            if (settings.FrequencyMins >= 1440)
            {
                response.Message = "Frequency minutes must be less than 1440.";
                return response;
            }

            if (string.IsNullOrEmpty(settings.S3AccessKey))
            {
                response.Message = "S3 access key is required.";
                return response;
            }

            if (string.IsNullOrEmpty(settings.S3BucketName))
            {
                response.Message = "S3 bucket name is required.";
                return response;
            }

            if (string.IsNullOrEmpty(settings.S3Region))
            {
                response.Message = "S3 region is required.";
                return response;
            }

            if (string.IsNullOrEmpty(settings.S3SecretKey))
            {
                response.Message = "S3 secret key is required.";
                return response;
            }

            if (!string.IsNullOrEmpty(settings.EmailsOnSuccess))
            {
                if (!IsEmailCsvValid(settings.EmailsOnSuccess))
                {
                    response.Message = "Success email field is invalid.";
                    return response;
                }
            }

            if (!string.IsNullOrEmpty(settings.EmailsOnFailure))
            {
                if (!IsEmailCsvValid(settings.EmailsOnFailure))
                {
                    response.Message = "Failure email field is invalid.";
                    return response;
                }
            }

            // check bucket exists
            if (!_deleteSyncService.IsValid().Result)
            {
                response.Message = "An error occurred connecting to Amazon S3 service. Check logs for more details.";
                return response;
            }

            response.IsValid = true;
            return response;
        }

        private bool IsEmailCsvValid(string emailCsv)
        {
            if (string.IsNullOrEmpty(emailCsv))
            {
                return false;
            }

            var split = emailCsv.Split(',');
            return split.All(i => Regex.IsMatch(i, @".+\@.+\..+"));
        }
    }

    public class ValidationResponseModel
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
    }
}
