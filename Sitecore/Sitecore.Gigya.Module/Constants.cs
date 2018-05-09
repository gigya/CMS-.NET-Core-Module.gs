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

        public class CmsFields
        {
            public const string UserId = "UserId";
        }

        public class Paths
        {
            public const string GlobalSettings = "/sitecore/system/Modules/Gigya/Global Settings";
            public const string SiteSettingsSuffix = "Gigya Settings/Gigya Settings";
        }

        public class Fields
        {
            public const string ApiKey = "API Key";
            public const string ApplicationKey = "Application Key";
            public const string ApplicationSecret = "Application Secret";
            public const string Language = "Language";
            public const string DebugMode = "Debug Mode";
            public const string DataCenter = "Data Center";
            public const string GlobalParameters = "Global Parameters";
            public const string EnableRaaS = "Enable RaaS";
            public const string EnableXdbSync = "Enable xDB Sync";
            public const string EnableMembershipProviderSync = "Enable Membership Provider Sync";
            public const string RedirectUrl = "Redirect URL";
            public const string LogoutUrl = "LogoutUrl";
            public const string MembershipMappingFields = "Membership Mapping Fields";
            public const string XdbMappingFields = "xDB Mapping Fields";
            public const string GigyaSessionType = "Gigya Session Type";
            public const string GigyaSessionDuration = "Gigya Session Duration";
        }
    }
}
