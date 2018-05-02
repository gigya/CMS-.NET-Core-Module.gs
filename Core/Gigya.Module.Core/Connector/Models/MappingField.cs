using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gigya.Module.Core.Connector.Models
{
    public class MappingField
    {
        public bool GigyaRequired { get; set; }
        public bool Required { get; set; }
        public string GigyaFieldName { get; set; }
        public string CmsFieldName { get; set; }
    }
}