using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.DS.Config
{
    [Serializable]
    public class GigyaDsSettingsContainer
    {
        public List<GigyaDsSettings> Sites { get; set; }
    }

    [Serializable]
    public class GigyaDsSettings
    {
        public string[] SiteId { get; set; }
        public GigyaDsMethod Method { get; set; }
        public List<GigyaDsMapping> Mappings { get; set; }
        public Dictionary<string, List<GigyaDsMapping>> MappingsByType { get; set; }
    }

    [Serializable]
    public enum GigyaDsMethod
    {
        Search = 0,
        Get = 1
    }

    [Serializable]
    public class GigyaDsMapping
    {
        public string CmsName { get; set; }
        public string GigyaName { get; set; }
        public string GigyaDsType { get; set; }
        public string GigyaFieldName { get; set; }
        public Custom Custom { get; set; }
    }

    [Serializable]
    public class Custom
    {
        public string Oid { get; set; }
    }
}
