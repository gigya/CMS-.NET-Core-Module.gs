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
        public const string StandardValuesName = "__Standard Values";

        public class Security
        {
            public const string AdminRole = "sitecore\\Gigya Major Admin";
        }

        public class DefaultSettings
        {
            public const string SessionTimeout = "1800";
            public const string DataCenter = "us1.gigya.com";
            public const string LanguageFallback = "en";
        }

        public class CmsFields
        {
            public const string UserId = "UserId";
            public const string FullName = "Full Name";
            public const string Email = "Email";
            public const string Comment = "Comment";
        }

        public class Ids
        {
            public static readonly ID GlobalSettings = new ID("{2EB49887-39E6-40FC-8EC5-3CBE241B2494}");
            public static readonly ID UserProperties = new ID("{1578C18A-C918-48EF-A496-4DCE4C5CD450}");
        }

        public class Paths
        {
            public const string SiteSettingsSuffix = "Gigya Settings";
        }

        public class Templates
        {
            public static readonly ID GigyaSettings = new ID("{CB5000AC-098F-420E-B973-D6AD423F2DAE}");
            public static readonly ID SitecoreDefaultUserProfile = new ID("{642C9A7E-EE31-4979-86F0-39F338C10AFB}");
            public static readonly ID MappingFieldFolder = new ID("{6FE4A2F7-B184-463F-A7CA-8471159E579A}");
            public static readonly ID MappingField = new ID("{A8ECE2D3-757C-4A0E-A184-DFAFC194C973}");
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
            public const string Profile = "Profile";

            public class MappingFields
            {
                public const string GigyaProperty = "Gigya Property";
                public const string SitecoreProperty = "Sitecore Property";
            }

            public class MappingFieldFolder
            {
                public const string Type = "Type";
            }
        }
    }
}
