using Gigya.Module.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gigya.Module.Data
{
	/// <summary>
	/// Class used to represent a Gigya Setting
	/// </summary>
	public class GigyaSitefinityModuleSettings
    {
        public Guid SiteId { get; set; }

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

        /// <summary>
        /// Initializes a new instance of the <see cref="GigyaModuleSettings"/> class.
        /// </summary>
        public GigyaSitefinityModuleSettings()
		{
		}
	}
}