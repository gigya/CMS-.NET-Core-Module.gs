using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Gigya.Module.Models
{
    public class GigyaBaseViewModel
    {
        public string Label { get; set; }
        public string LoggedInUrl { get; set; }
        public string ScreenSet { get; set; }
        public string MobileScreenSet { get; set; }
        public string StartScreen { get; set; }
        public string ContainerId { get; set; }
        public string GeneratedContainerId { get; set; }
    }
}