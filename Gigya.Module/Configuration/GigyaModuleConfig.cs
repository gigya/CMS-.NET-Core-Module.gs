using System;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Localization;

namespace Gigya.Module.Configuration
{
    /// <summary>
    /// Sitefinity configuration section.
    /// </summary>
    [ObjectInfo(Title = "Gigya Settings Module Config", Description = "Gigya Module Settings Config")]
    public class GigyaModuleConfig : ConfigSection
    {
        //[ObjectInfo(Title = "Text", Description = "This is a sample string field.")]
        [ConfigurationProperty("SiteId")]
        [Browsable(false)]
        public Guid SiteId { get; set; }

        [ConfigurationProperty("ApiKey")]
        [Browsable(true)]
        public string ApiKey { get; set; }

        [ConfigurationProperty("ApplicationKey")]
        [Browsable(true)]
        public string ApplicationKey { get; set; }

        [ConfigurationProperty("ApplicationSecret")]
        [Browsable(true)]
        public string ApplicationSecret { get; set; }

        [ConfigurationProperty("Language")]
        [Browsable(true)]
        public string Language { get; set; }

        [ConfigurationProperty("DebugMode")]
        [Browsable(true)]
        public bool DebugMode { get; set; }

        [ConfigurationProperty("DataCenter")]
        [Browsable(true)]
        public string DataCenter { get; set; }

        [ConfigurationProperty("EnableRaas")]
        [Browsable(true)]
        public bool EnableRaas { get; set; }

        [ConfigurationProperty("RedirectUrl")]
        [Browsable(true)]
        public string RedirectUrl { get; set; }

        [ConfigurationProperty("MappingFields")]
        [Browsable(true)]
        public string MappingFields { get; set; }

        [ConfigurationProperty("GlobalParameters")]
        [Browsable(true)]
        public string GlobalParameters { get; set; }
    }
}