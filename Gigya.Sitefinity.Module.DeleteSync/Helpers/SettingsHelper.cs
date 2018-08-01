using Gigya.Module.DeleteSync.Models;
using Gigya.Sitefinity.Module.DeleteSync.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Sitefinity.Module.DeleteSync.Helpers
{
    public class SettingsHelper
    {
        public DeleteSyncSettings GetSettings()
        {
            using (var context = GigyaDeleteSyncContext.Get())
            {
                var settings = context.Settings.FirstOrDefault();

                if (settings != null && !string.IsNullOrEmpty(settings.S3SecretKey))
                {
                    settings.S3SecretKey = Gigya.Module.Core.Connector.Encryption.Encryptor.Decrypt(settings.S3SecretKey);
                }

                return settings;
            }
        }
    }
}
