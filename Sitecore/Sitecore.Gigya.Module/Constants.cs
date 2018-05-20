using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Gigya.Module
{
    public static class Constants
    {
        public const string ModuleVersion = "6";
        public const string GlobalSettingsId = "{3d64effe-6e9f-45a7-a716-14d588b62d44}";
        public const string EncryptionPrefix = "___ENCRYPTED___";

        public class DefaultSettings
        {
            public const string SessionTimeout = "1800";
            public const string DataCenter = "us1.gigya.com";
            public const string LanguageFallback = "en";
        }

        public class CmsFields
        {
            public const string UserId = "UserId";
        }

        public class Paths
        {
            public const string GlobalSettings = "/sitecore/system/Modules/Gigya/Global Settings";
            public const string SiteSettingsSuffix = "Gigya Settings/Gigya Settings";
        }

        public class Templates
        {
            public static readonly ID GigyaSettings = new ID("{CB5000AC-098F-420E-B973-D6AD423F2DAE}");
        }

        public class Dictionary
        {
            public const string Domain = "Gigya";
            public const string GenericError = "GenericError";
        }

        public class Fields
        {
            public const string ApiKey = "API Key";
            public const string ApplicationKey = "Application Key";
            public const string ApplicationSecret = "Application Secret";
            public const string Language = "Language";
            //public const string Language = "Language Fallback";
            public const string DebugMode = "Debug Mode";
            public const string DataCenter = "Data Center";
            public const string GlobalParameters = "Global Parameters";
            public const string EnableRaaS = "Enable RaaS";
            public const string EnableXdbSync = "Enable xDB Sync";
            public const string EnableMembershipProviderSync = "Enable Membership Provider Sync";
            public const string RedirectUrl = "Redirect URL";
            public const string LogoutUrl = "Logout URL";
            public const string MembershipMappingFields = "Membership Mapping Fields";
            public const string XdbMappingFields = "xDB Mapping Fields";
            public const string GigyaSessionType = "Gigya Session Type";
            public const string GigyaSessionDuration = "Gigya Session Duration";
        }
    }
}
