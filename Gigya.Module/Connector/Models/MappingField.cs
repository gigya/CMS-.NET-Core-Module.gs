using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gigya.Module.Connector.Models
{
    public class MappingField
    {
        public bool Required { get; set; }
        public string GigyaFieldName { get; set; }
        public string SitefinityFieldName { get; set; }
    }
}