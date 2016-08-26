using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.UnitTests.Selenium
{
    public static class Config
    {
        public static readonly string Site1BaseURL = ConfigurationManager.AppSettings["Site1BaseUrl"] ?? "http://gigya.local/";
        public static readonly string Site2BaseURL = ConfigurationManager.AppSettings["Site2BaseUrl"] ?? "http://gigya2.local/";

        public static readonly string AdminUsername = ConfigurationManager.AppSettings["CmsUserName"] ?? "admin";
        public static readonly string AdminPassword = ConfigurationManager.AppSettings["CmsPassword"] ?? "aa234567";
        public static readonly string AdminFirstName = ConfigurationManager.AppSettings["CmsFirstName"] ?? "Admin";
        public static readonly string AdminLastName = ConfigurationManager.AppSettings["CmsLastName"] ?? "Admin";
        public static readonly string AdminEmail = ConfigurationManager.AppSettings["CmsEmail"] ?? "admin@purestone.co.uk";

        public static readonly string NonAdminUsername = ConfigurationManager.AppSettings["CmsNonAdminUserName"] ?? "nonadmin";
        public static readonly string NonAdminPassword = ConfigurationManager.AppSettings["CmsNonAdminPassword"] ?? "aa234567";
        public static readonly string NonAdminFirstName = ConfigurationManager.AppSettings["CmsNonAdminFirstName"] ?? "Non";
        public static readonly string NonAdminLastName = ConfigurationManager.AppSettings["CmsNonAdminLastName"] ?? "Admin";
        public static readonly string NonAdminEmail = ConfigurationManager.AppSettings["CmsNonAdminEmail"] ?? "nonadmin@purestone.co.uk";

        public static readonly string LicensePath = ConfigurationManager.AppSettings["SitefinityLicensePath"];

        public static readonly string Site1ApiKey = ConfigurationManager.AppSettings["Site1ApiKey"];
        public static readonly string Site2ApiKey = ConfigurationManager.AppSettings["Site2ApiKey"];

        public static readonly string Site1ApplicationKey = ConfigurationManager.AppSettings["Site1ApplicationKey"];
        public static readonly string Site2ApplicationKey = ConfigurationManager.AppSettings["Site2ApplicationKey"];

        public static readonly string Site1DataCenter = ConfigurationManager.AppSettings["Site1DataCenter"] ?? "EU";
        public static readonly string Site2DataCenter = ConfigurationManager.AppSettings["Site2DataCenter"] ?? "EU";

        public static readonly string Site1LangFallback = ConfigurationManager.AppSettings["Site1LangFallback"] ?? "English (default)";
        public static readonly string Site2LangFallback = ConfigurationManager.AppSettings["Site2LangFallback"] ?? "English (default)";

        public static readonly string Site1ApplicationSecret = ConfigurationManager.AppSettings["Site1ApplicationSecret"];
        public static readonly string Site2ApplicationSecret = ConfigurationManager.AppSettings["Site2ApplicationSecret"];

        public static readonly string SitefinityRootPath = ConfigurationManager.AppSettings["SitefinityRootPath"];

        public static readonly string UmbracoRootPath = ConfigurationManager.AppSettings["UmbracoRootPath"];
        public static readonly string UmbracoPackagePath = ConfigurationManager.AppSettings["UmbracoPackagePath"];
    }
}
