using Gigya.Module.Core.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Gigya.Module.Core.Connector.Helpers
{
    public class GigyaLanguageHelper
    {
        public static Dictionary<string, string> Languages;

        /// <summary>
        /// Gets the Gigya language to use.
        /// </summary>
        /// <returns>A culture code.</returns>
        public string Language(IGigyaModuleSettings settings, CultureInfo currentCulture)
        {
            if (settings.Language != "auto")
            {
                return settings.Language;
            }

            var language = currentCulture.Name.ToLowerInvariant();
            if (Languages.ContainsKey(language))
            {
                return language;
            }

            // attempt to find non specific culture
            language = currentCulture.TwoLetterISOLanguageName.ToLowerInvariant();
            if (Languages.ContainsKey(language))
            {
                return language;
            }

            return settings.LanguageFallback;
        }

        static GigyaLanguageHelper()
        {
            Languages = new Dictionary<string, string>();

            Languages.Add("en", "English (default)");
            Languages.Add("ar", "Arabic");
            Languages.Add("br", "Bulgarian");
            Languages.Add("ca", "Catalan");
            Languages.Add("zh-cn", "Chinese (Mandarin)");
            Languages.Add("zh-hk", "Chinese (Hong Kong)");
            Languages.Add("zh-tw", "Chinese (Taiwan)");
            Languages.Add("hr", "Croatian");
            Languages.Add("cs", "Czech");
            Languages.Add("da", "Danish");
            Languages.Add("nl", "Dutch");
            Languages.Add("nl-inf", "Dutch Informal");
            Languages.Add("fi", "Finnish");
            Languages.Add("fr", "French");
            Languages.Add("fr-inf", "French Informal");
            Languages.Add("de", "German");
            Languages.Add("de-inf", "German Informal");
            Languages.Add("el", "Greek");
            Languages.Add("he", "Hebrew");
            Languages.Add("hu", "Hungarian");
            Languages.Add("id", "Indonesian (Bahasa)");
            Languages.Add("it", "Italian");
            Languages.Add("ja", "Japanese");
            Languages.Add("ko", "Korean");
            Languages.Add("ms", "Malay");
            Languages.Add("no", "Norwegian");
            Languages.Add("fa", "Persian (Farsi)");
            Languages.Add("pl", "Polish");
            Languages.Add("pt", "Portuguese");
            Languages.Add("pt-br", "Portuguese (Brazil)");
            Languages.Add("ro", "Romanian");
            Languages.Add("ru", "Russian");
            Languages.Add("sr", "Serbian (Cyrillic)");
            Languages.Add("sk", "Slovak");
            Languages.Add("sl", "Slovenian");
            Languages.Add("es", "Spanish");
            Languages.Add("es-inf", "Spanish Informal");
            Languages.Add("es-mx", "Spanish (Lat-Am)");
            Languages.Add("sv", "Swedish");
            Languages.Add("tl", "Tagalog");
            Languages.Add("th", "Thai");
            Languages.Add("tr", "Turkish");
            Languages.Add("uk", "Ukrainian");
            Languages.Add("vi", "Vietnamese");
        }
    }
}