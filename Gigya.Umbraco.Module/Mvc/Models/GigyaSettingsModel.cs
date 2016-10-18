using Gigya.Module.Core.Connector.Enums;
using Gigya.Module.Core.Connector.Models;
using Gigya.Module.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Umbraco.Module.Mvc.Models
{
    public class GigyaSettingsModel
    {
        public int Id { get; set; }
        public string SiteName { get; set; }
        [Required]
        public string ApiKey { get; set; }
        [Required]
        public string ApplicationKey { get; set; }
        public string ApplicationSecret { get; set; }
        public string ApplicationSecretMasked { get; set; }
        public bool CanViewApplicationSecret { get; set; }
        [Required]
        public GigyaLanguageModel Language { get; set; }
        public GigyaLanguageModel LanguageFallback { get; set; }
        public string LanguageOther { get; set; }
        public bool DebugMode { get; set; }
        [Required]
        public string DataCenter { get; set; }
        public string DataCenterOther { get; set; }
        public bool EnableRaas { get; set; }
        public string RedirectUrl { get; set; }
        public string LogoutUrl { get; set; }
        public List<MappingField> MappingFields { get; set; }
        public string GlobalParameters { get; set; }
        public GigyaSessionProvider SessionProvider { get; set; }
        public int SessionTimeout { get; set; }
        public bool Inherited { get; set; }
    }
}
