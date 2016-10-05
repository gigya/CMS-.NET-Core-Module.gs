using Gigya.Module.Core.Connector.Models;
using Gigya.Module.Core.Data;
using Gigya.Module.DS.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Umbraco.Module.DS.Mvc.Models
{
    public class GigyaDsSettingsViewModel
    {
        public int Id { get; set; }
        public GigyaDsMethod Method { get; set; }
        [Required]
        public List<GigyaDsMappingViewModel> Mappings { get; set; }
        public bool Inherited { get; set; }
    }

    public class GigyaDsMappingViewModel
    {
        public string CmsName { get; set; }
        public string GigyaName { get; set; }
        public string Oid { get; set; }
    }

    public class GigyaDsSettingsResponseModel
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public GigyaDsSettingsViewModel Settings { get; set; }
    }
}
