using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Umbraco.Module.Mvc.Models
{
    public class GigyaSettingsResponseModel
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public GigyaSettingsModel Settings { get; set; }
    }
}
