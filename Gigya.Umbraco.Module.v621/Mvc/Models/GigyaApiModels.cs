using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Umbraco.Module.v621.Mvc.Models
{
    public class GigyaSettingsApiResponseModel
    {
        public GigyaSettingsModel Settings { get; set; }
        public GigyaConfigModel Data { get; set; }
    }
}
