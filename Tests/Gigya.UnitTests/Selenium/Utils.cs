using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Gigya.UnitTests.Selenium
{
    public static class Utils
    {
        public static void AddEncryptionKey(string rootPath)
        {
            XmlDocument webConfigDoc = new XmlDocument();
            webConfigDoc.Load(Path.Combine(rootPath, "web.config"));

            // add encryption key to web.config
            var encryptionElem = webConfigDoc.SelectSingleNode("/configuration/appSettings/add[@key='Gigya.Encryption.Key']");
            if (encryptionElem == null)
            {
                var appSettings = webConfigDoc.SelectSingleNode("/configuration/appSettings");

                encryptionElem = webConfigDoc.CreateElement("add");
                var keyAttribute = webConfigDoc.CreateAttribute("key");
                keyAttribute.Value = "Gigya.Encryption.Key";

                var valueAttribute = webConfigDoc.CreateAttribute("value");
                valueAttribute.Value = Config.EncryptionKey;

                encryptionElem.Attributes.Append(keyAttribute);
                encryptionElem.Attributes.Append(valueAttribute);

                appSettings.AppendChild(encryptionElem);

                webConfigDoc.Save(Path.Combine(rootPath, "web.config"));
            }
        }
    }
}
