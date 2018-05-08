using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Gigya.Module.Models
{
    public class GigyaLogoutRenderingModel : GigyaBaseRenderingModel
    {
        public string LoggedOutUrl { get; set; }
    }
}