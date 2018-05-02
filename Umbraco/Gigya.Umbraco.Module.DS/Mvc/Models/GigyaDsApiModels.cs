using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Umbraco.Module.DS.Mvc.Models
{
    public class GigyaDsSettingsApiResponseModel
    {
        public GigyaDsSettingsViewModel Settings { get; set; }
        public GigyaDsConfigViewModel Data { get; set; }
    }
}
