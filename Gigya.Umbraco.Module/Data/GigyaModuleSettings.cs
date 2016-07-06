using Gigya.Module.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Gigya.Umbraco.Module.Data
{
    public class GigyaModuleSettings : IGigyaModuleSettings
    {
        public object Id { get; set; }

        public string ApiKey { get; set; }

        public string ApplicationKey { get; set; }

        public string ApplicationSecret { get; set; }

        public string Language { get; set; }

        public string LanguageFallback { get; set; }

        public bool DebugMode { get; set; }

        public string DataCenter { get; set; }

        public bool EnableRaas { get; set; }

        public string RedirectUrl { get; set; }

        public string LogoutUrl { get; set; }

        public string MappingFields { get; set; }

        public string GlobalParameters { get; set; }

        public int SessionTimeout { get; set; }
    }
}
