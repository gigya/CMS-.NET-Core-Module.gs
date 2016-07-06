using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.Mvc.Models
{
    public class GigyaSettingsViewModel
    {
        public string ApiKey { get; set; }
        public dynamic Settings { get; set; }
        public string SettingsJson { get; set; }
        public bool DebugMode { get; set; }
        public bool RenderScript { get; set; }
        public string GigyaScriptPath { get; set; }
    }
}
