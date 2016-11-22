using Gigya.Module.DS.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.OpenAccess;

namespace Gigya.Sitefinity.Module.DS.Data
{
    public class GigyaSitefinityModuleDsSettings
    {
        public Guid SiteId { get; set; }
        public GigyaDsMethod Method { get; set; }
        public IList<GigyaSitefinityDsMapping> Mappings { get; set; }
    }
    
    public class GigyaSitefinityDsMapping
    {
        public int Id { get; set; }
        public GigyaSitefinityModuleDsSettings Settings { get; set; }
        public Guid DsSettingId { get; set; }
        public string CmsName { get; set; }
        public string GigyaName { get; set; }
        public string Oid { get; set; }
    }
}
