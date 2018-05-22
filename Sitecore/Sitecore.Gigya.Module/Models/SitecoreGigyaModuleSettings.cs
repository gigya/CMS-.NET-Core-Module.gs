using Gigya.Module.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Gigya.Module.Models
{
    public class SitecoreGigyaModuleSettings : GigyaModuleSettings
    {
        public bool EnableMembershipSync { get; set; }
        public bool EnableXdb { get; set; }
    }
}