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

        public static readonly string LicensePath = ConfigurationManager.AppSettings["SitefinityLicensePath"];
    }
}
