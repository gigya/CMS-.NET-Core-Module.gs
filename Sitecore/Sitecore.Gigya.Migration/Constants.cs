using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Gigya.Migration
{
    public class Constants
    {
        public class Ids
        {
            public static readonly ID GlobalSettings = new ID(Sitecore.Gigya.Module.Constants.GlobalSettingsId);
            public static readonly ID ShareProviderSettingsItemId = new ID("{50B70327-1F66-4EA2-9749-F92B736172F4}");
            public static readonly ID xDbPersonalMapping = new ID("{CFF292C4-17AA-4C5F-8DC8-44655126BA39}");
            public static readonly ID MembershipMapping = new ID("{1F2154B9-3991-49C2-A0A7-3733422729E2}");
            public static readonly ID MappingFieldTemplate = new ID("{A8ECE2D3-757C-4A0E-A184-DFAFC194C973}");
            public static readonly ID DataCenterFolder = new ID("{7D2827F8-A73F-49D7-94C7-3187F90E9625}");
            public static readonly ID GigyaSettings = new ID("{CB5000AC-098F-420E-B973-D6AD423F2DAE}");
            public static readonly ID UserTemplateId = new ID("{642C9A7E-EE31-4979-86F0-39F338C10AFB}");
            public static readonly ID TemplateField = new ID("{455A3E98-A627-4B40-8035-E683A0331AC7}");
        }

        public class Fields
        {
            public const string ApiKey = "API Key";
            public const string ApplicationKey = "Application Key";
            public const string ApplicationSecret = "Application Secret";
            public const string DataCenter = "Data Center";
            public const string GlobalParameters = "Global Parameters";
            public const string Profile = "Profile";
            public const string Parent = "Settings Parent";

            public class MappingFields
            {
                public const string GigyaProperty = "Gigya Property";
                public const string SitecoreProperty = "Sitecore Property";
            }

            public class MappingFieldFolder
            {
                public const string Type = "Type";
            }

            public class FacetFolder
            {
                public const string Name = "Facet Name";
            }

            public struct PluginConfigItemFieldNames
            {
                public const string ShareButtonName = "Share Button Name";
                public const string ShareProviders = "Share Providers";
            }

            public class PersonalFacet
            {
                public const string FirstName = "FirstName";
                public const string Surname = "Surname";
                public const string Gender = "Gender";
            }
        }
    }
}
