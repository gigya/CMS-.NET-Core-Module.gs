using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Gigya.Module.Models
{
    public class SessionValidationModel
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
    }
}