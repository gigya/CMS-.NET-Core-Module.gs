using Gigya.Module.Core.Connector.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gigya.Module.Core.Data
{
	/// <summary>
	/// Interface used to represent a Gigya Setting
	/// </summary>
	public interface IGigyaModuleSettings
	{
        object Id { get; set; }

        string ApiKey { get; set; }

        string ApplicationKey { get; set; }

        string ApplicationSecret { get; set; }

        string Language { get; set; }

        string LanguageFallback { get; set; }

        bool DebugMode { get; set; }

        string DataCenter { get; set; }

        bool EnableRaas { get; set; }

        string RedirectUrl { get; set; }

        string LogoutUrl { get; set; }

        string MappingFields { get; set; }

        string GlobalParameters { get; set; }

        int SessionTimeout { get; set; }

        GigyaSessionProvider SessionProvider { get; set; }
    }
}