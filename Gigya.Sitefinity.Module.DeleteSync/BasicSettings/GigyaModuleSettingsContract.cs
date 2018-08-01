using System;
using System.Linq;
using System.Runtime.Serialization;
using Telerik.Sitefinity.Security.Claims;
using Gigya.Module.Core.Connector.Encryption;
using Gigya.Module.Core.Connector.Helpers;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.Connector.Logging;
using Gigya.Sitefinity.Module.DeleteSync.Data;
using Gigya.Sitefinity.Module.DeleteSync.Helpers;
using Gigya.Sitefinity.Module.DeleteSync.Tasks;
using Gigya.Module.DeleteSync.Models;
using Gigya.Module.DeleteSync.Helpers;
using Gigya.Module.DeleteSync;
using Gigya.Module.DeleteSync.Providers;
using Gigya.Sitefinity.Module.DeleteSync.Models;

namespace Gigya.Sitefinity.Module.DeleteSync.BasicSettings
{
    [DataContract]
    public class GigyaModuleSettingsContract : IGigyaSettingsDataContract
    {
        private Logger _logger;
        private Logger Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = new Logger(new SitefinityLogger());
                }
                return _logger;
            }
        }

        #region Data Members

        [DataMember]
        public DeleteSyncAction Action { get; set; }
        [DataMember]
        public bool Enabled { get; set; }
        [DataMember]
        public int FrequencyMins { get; set; }
        [DataMember]
        public string EmailsOnSuccess { get; set; }
        [DataMember]
        public string EmailsOnFailure { get; set; }
        [DataMember]
        public string S3BucketName { get; set; }
        [DataMember]
        public string S3AccessKey { get; set; }
        [DataMember]
        public string S3SecretKey { get; set; }
        [DataMember]
        public string S3SecretKeyMasked { get; set; }
        [DataMember]
        public string S3ObjectKeyPrefix { get; set; }
        [DataMember]
        public string S3Region { get; set; }
        [DataMember]
        public int MaxAttempts { get; set; }
        [DataMember]
        public bool CanViewSecretKey { get; set; }

        /// <summary>
        /// This field is required so we can poll on the client and detect if the service has responded.
        /// </summary>
        [DataMember]
        public string LoadedField { get; set; }

        #endregion

        #region ISettingsDataContract Implementation

        public void LoadDefaults(bool forEdit = false)
        {
            Load();
        }

        /// <summary>
        /// Get values from module config and load them into data members
        /// </summary>
        /// <param name="forEdit"></param>
        public void Load()
        {
            using (var context = GigyaDeleteSyncContext.Get())
            {
                // get settings for site or global settings or new settings
                var settings = context.Settings.FirstOrDefault() ?? new SitefinityDeleteSyncSettings();

                // map settings to this
                this.Action = settings.Action;
                this.EmailsOnFailure = settings.EmailsOnFailure;
                this.EmailsOnSuccess = settings.EmailsOnSuccess;
                this.Enabled = settings.Enabled;
                this.FrequencyMins = settings.FrequencyMins;
                this.S3AccessKey = settings.S3AccessKey;
                this.S3BucketName = settings.S3BucketName;
                this.S3ObjectKeyPrefix = settings.S3ObjectKeyPrefix;
                this.S3Region = settings.S3Region;
                this.MaxAttempts = settings.MaxAttempts;

                // check if user can view application secret
                var identity = ClaimsManager.GetCurrentIdentity();
                this.CanViewSecretKey = identity.IsAuthenticated && Gigya.Module.Connector.Admin.Roles.HasRole(identity);

                if (CanViewSecretKey && !string.IsNullOrEmpty(settings.S3SecretKey) && Encryptor.IsConfigured)
                {
                    var key = TryDecryptApplicationSecret(settings.S3SecretKey, false);
                    if (!string.IsNullOrEmpty(key))
                    {
                        this.S3SecretKeyMasked = StringHelper.MaskInput(key, "*", 2, 2);
                    }
                }

                this.LoadedField = "Loaded";
            }
        }

        public void SaveDefaults()
        {
            Save();
        }

        /// <summary>
        /// Save values from basic settings page
        /// </summary>
        public void Save()
        {
            using (var context = GigyaDeleteSyncContext.Get())
            {
                // get settings to update
                var settings = context.Settings.FirstOrDefault() ?? new SitefinityDeleteSyncSettings();

                // update all fields
                settings.Action = this.Action;
                settings.EmailsOnFailure = this.EmailsOnFailure;
                settings.EmailsOnSuccess = this.EmailsOnSuccess;
                settings.Enabled = this.Enabled;
                settings.FrequencyMins = this.FrequencyMins;
                settings.S3AccessKey = this.S3AccessKey;
                settings.S3BucketName = this.S3BucketName;
                settings.S3ObjectKeyPrefix = this.S3ObjectKeyPrefix;
                settings.S3Region = this.S3Region;
                settings.MaxAttempts = this.MaxAttempts;

                // check if user can view application secret
                if (!string.IsNullOrEmpty(this.S3SecretKey))
                {
                    var identity = ClaimsManager.GetCurrentIdentity();
                    var canViewApplicationSecret = identity.IsAuthenticated && Gigya.Module.Connector.Admin.Roles.HasRole(identity);
                    if (canViewApplicationSecret)
                    {
                        if (!Encryptor.IsConfigured)
                        {
                            throw new ArgumentException("Encryption key not specified. Refer to installation guide.");
                        }

                        settings.S3SecretKey = Encryptor.Encrypt(S3SecretKey.Trim());
                    }
                }

                var deleteProvider = new AmazonProvider(settings.S3AccessKey, settings.S3SecretKey, settings.S3BucketName, settings.S3ObjectKeyPrefix, settings.S3Region, Logger);
                var deleteService = new DeleteSyncService(deleteProvider);
                var validationHelper = new ValidationHelper(deleteService);
                var mappedSettings = Mapper.Map(settings);
                var validationResponse = validationHelper.Validate(mappedSettings);
                if (!validationResponse.IsValid)
                {
                    throw new ArgumentException(validationResponse.Message);
                }
                
                context.Add(settings);
                context.SaveChanges();

                // schedule job
                if (settings.Enabled)
                {
                    DeleteSyncTask.ScheduleTask(DateTime.Now.AddMinutes(settings.FrequencyMins));
                }
            }
        }

        private string TryDecryptApplicationSecret(string secret, bool throwOnException = true)
        {
            try
            {
                return Encryptor.Decrypt(secret);
            }
            catch (Exception e)
            {
                Logger.Error("Couldn't decrypt secret.", e);
                if (throwOnException)
                {
                    throw new ArgumentException("Couldn't decrypt secret. Please enter it again.");
                }
            }
            return null;
        }

        #endregion
    }
}