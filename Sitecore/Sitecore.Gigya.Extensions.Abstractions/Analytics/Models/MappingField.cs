using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Gigya.Extensions.Abstractions.Analytics.Models
{
    public class MappingField
    {
        public string GigyaFieldName { get; set; }
        public string CmsFieldName { get; set; }
    }

    public class MappingFieldGroup
    {
        public string FacetName { get; set; }
        public List<MappingField> Fields { get; set; }
    }
}
