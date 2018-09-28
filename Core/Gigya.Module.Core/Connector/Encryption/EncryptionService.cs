using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web.Hosting;

namespace Gigya.Module.Core.Connector.Encryption
{
    public class EncryptionService : IEncryptionService
    {
        private readonly string _key = ConfigurationManager.AppSettings["Gigya.Encryption.Key"];
        private readonly string _keyLocation = ConfigurationManager.AppSettings["Gigya.Encryption.KeyLocation"];
        private readonly byte[] _salt = Encoding.ASCII.GetBytes(ConfigurationManager.AppSettings["Gigya.Encryption.Salt"] ?? "lkjslkfj!sldkflkj£$EdfS34!£$XzaEdfjkrm");

        private static readonly Lazy<EncryptionService> _instance = new Lazy<EncryptionService>(() => new EncryptionService());

        public static EncryptionService Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private EncryptionService()
        {
            if (!string.IsNullOrEmpty(_keyLocation))
            {
                if (_keyLocation.StartsWith("~/"))
                {
                    _keyLocation = HostingEnvironment.MapPath(_keyLocation);
                }

                if (File.Exists(_keyLocation))
                {
                    // don't need a try catch as if we can't read the key we can't continue so it's better to throw the error
                    _key = File.ReadAllText(_keyLocation);
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
