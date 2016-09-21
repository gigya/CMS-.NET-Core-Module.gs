using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.DS.Config
{
    [Serializable]
    public class GigyaDsSettings
    {
        public GigyaDsMethod Method { get; set; }
        public List<GigyaDsMapping> Mappings { get; set; }
        public Dictionary<string, List<GigyaDsMapping>> MappingsByType { get; set; }
    }

    [Serializable]
    public enum GigyaDsMethod
    {
        Search,
        Get
    }

    [Serializable]
    public class GigyaDsMapping
    {
        //public string CmsName { get; set; }
        //public string CmsType { get; set; }
        public string GigyaName { get; set; }
        public string GigyaDsType { get; set; }
        public string GigyaFieldName { get; set; }
        //public string GigyaType { get; set; }
        public Custom Custom { get; set; }
    }

    [Serializable]
    public class Custom
    {
        public string Oid { get; set; }
    }
}
