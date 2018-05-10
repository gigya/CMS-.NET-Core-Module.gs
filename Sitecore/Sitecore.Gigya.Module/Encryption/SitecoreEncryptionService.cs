using Gigya.Module.Core.Connector.Encryption;
using Sitecore.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace Sitecore.Gigya.Module.Encryption
{
    public class SitecoreEncryptionService : IEncryptionService
    {
        private readonly string _key;
        private readonly byte[] _salt;

        private static readonly Lazy<SitecoreEncryptionService> _instance = new Lazy<SitecoreEncryptionService>(() => new SitecoreEncryptionService());

        public static SitecoreEncryptionService Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private SitecoreEncryptionService()
        {
            _key = Settings.GetSetting("Sitecore.Gigya.Module.Encryption.Key");
            _salt = Encoding.ASCII.GetBytes(Settings.GetSetting("Sitecore.Gigya.Module.Encryption.Salt", "{cb4c5f49-0513-4a67-9a82-6e73db3ca185}{bcb93965-4483-419d-8ead-165d4b9d09ed}{062e5385-09e3-46b6-b375-292bfdfb52df}"));

            var keyLocation = Settings.GetSetting("Sitecore.Gigya.Module.Encryption.KeyLocation");

            if (!string.IsNullOrEmpty(keyLocation))
            {
                if (keyLocation.StartsWith("~/"))
                {
                    keyLocation = HostingEnvironment.MapPath(keyLocation);
                }

                if (File.Exists(keyLocation))
                {
                    // don't need a try catch as if we can't read the key we can't continue so it's better to throw the error
                    _key = File.ReadAllText(keyLocation);
                }
            }
        }

        public bool IsConfigured => !string.IsNullOrEmpty(_key);

        public string Decrypt(string cipherText)
        {
            return Encryptor.DecryptStringAES(cipherText, _key, _salt);
        }

        public string Encrypt(string plainText)
        {
            return Encryptor.EncryptStringAES(plainText, _key, _salt);
        }
    }
}