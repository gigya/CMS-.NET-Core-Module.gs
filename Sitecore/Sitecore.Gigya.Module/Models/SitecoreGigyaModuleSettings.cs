using Gigya.Module.Core.Connector.Models;
using Gigya.Module.Core.Data;
using Sitecore.Gigya.Extensions.Abstractions.Analytics.Models;
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
        public MappingFieldGroup MappedXdbMappingFields { get; set; }
        public string ProfileId { get; set; }
    }
}