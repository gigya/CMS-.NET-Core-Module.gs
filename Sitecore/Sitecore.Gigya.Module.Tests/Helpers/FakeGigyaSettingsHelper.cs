using Sitecore.Gigya.Module.Helpers;
using Sitecore.Gigya.Module.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Gigya.Module.Tests.Helpers
{
    public class FakeGigyaSettingsHelper : GigyaSettingsHelper
    {
        public SitecoreGigyaModuleSettings Settings { get; private set; }

        public FakeGigyaSettingsHelper() : base()
        {
            Settings = DefaultSettings();
        }

        protected override string ReadVersionFile()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "sitecore/shell/sitecore.version.xml");
            var versionInfo = File.ReadAllText(path);
            return versionInfo;
        }

        private SitecoreGigyaModuleSettings DefaultSettings()
        {
            return new SitecoreGigyaModuleSettings
            {
                ApiKey = "3_qkAT5OcGyvYpkjc_VF6-OfoeTKGk4T_jVwjFF9f5TQzoAg-mH8SBsjQi1srdsOm6",
                ApplicationKey = "ABPcVRLxt+1u",
                ApplicationSecret = "rH7ZVYbTaodksq6u/JPI6OBe/rT/IZmN",
                Language = "auto",
                LanguageFallback = "en",
                DebugMode = true,
                DataCenter = "eu1.gigya.com",
                EnableRaas = true
            };
        }
    }
}
