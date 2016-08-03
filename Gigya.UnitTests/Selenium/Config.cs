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

        public static readonly string AdminUsername = ConfigurationManager.AppSettings["SitefinityUserName"] ?? "admin";
        public static readonly string AdminPassword = ConfigurationManager.AppSettings["SitefinityPassword"] ?? "aa234567";
        public static readonly string AdminFirstName = ConfigurationManager.AppSettings["SitefinityFirstName"] ?? "Admin";
        public static readonly string AdminLastName = ConfigurationManager.AppSettings["SitefinityLastName"] ?? "Admin";
        public static readonly string AdminEmail = ConfigurationManager.AppSettings["SitefinityEmail"] ?? "admin@purestone.co.uk";

        public static readonly string LicensePath = ConfigurationManager.AppSettings["SitefinityLicensePath"];

        public static readonly string Site1ApiKey = ConfigurationManager.AppSettings["Site1ApiKey"];
        public static readonly string Site2ApiKey = ConfigurationManager.AppSettings["Site2ApiKey"];

        public static readonly string Site1ApplicationKey = ConfigurationManager.AppSettings["Site1ApplicationKey"];
        public static readonly string Site2ApplicationKey = ConfigurationManager.AppSettings["Site2ApplicationKey"];

        public static readonly string Site1DataCenter = ConfigurationManager.AppSettings["Site1DataCenter"] ?? "EU";
        public static readonly string Site2DataCenter = ConfigurationManager.AppSettings["Site2DataCenter"] ?? "EU";

        public static readonly string Site1ApplicationSecret = ConfigurationManager.AppSettings["Site1ApplicationSecret"];
        public static readonly string Site2ApplicationSecret = ConfigurationManager.AppSettings["Site2ApplicationSecret"];
    }
}
