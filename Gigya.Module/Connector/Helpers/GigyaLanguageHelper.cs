using Gigya.Module.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Services;

namespace Gigya.Module.Connector.Helpers
{
    public static class GigyaLanguageHelper
    {
        private static Dictionary<string, bool> _languages;

        /// <summary>
        /// Gets the Gigya language to use.
        /// </summary>
        /// <returns>A culture code.</returns>
        public static string Language(GigyaModuleSettings settings)
        {
            if (settings.Language != "auto")
            {
                return settings.Language;
            }

            var language = CultureInfo.CurrentUICulture.Name.ToLowerInvariant();
            var currentSite = SystemManager.CurrentContext.CurrentSite;
            var cultures = currentSite.PublicContentCultures;
            if (cultures != null && cultures.Length == 1 && !string.IsNullOrEmpty(currentSite.DefaultCulture))
            {
                language = currentSite.DefaultCulture.ToLowerInvariant();
            }

            language = language.ToLowerInvariant();
            if (_languages.ContainsKey(language))
            {
                return language;
            }

            // attempt to non specific culture
            language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant();
            if (_languages.ContainsKey(language))
            {
                return language;
            }

            return settings.LanguageFallback;
        }

        static GigyaLanguageHelper()
        {
            using (var context = GigyaContext.Get())
            {
                _languages = context.Languages.ToDictionary(i => i.Language, j => true);

                if (!_languages.Any())
                {
                    context.Add(new GigyaLanguage { Language = "en" });
                    context.Add(new GigyaLanguage { Language = "ar" });
                    context.Add(new GigyaLanguage { Language = "br" });
                    context.Add(new GigyaLanguage { Language = "ca" });
                    context.Add(new GigyaLanguage { Language = "zh-cn" });
                    context.Add(new GigyaLanguage { Language = "zh-hk" });
                    context.Add(new GigyaLanguage { Language = "zh-tw" });
                    context.Add(new GigyaLanguage { Language = "hr" });
                    context.Add(new GigyaLanguage { Language = "cs" });
                    context.Add(new GigyaLanguage { Language = "da" });
                    context.Add(new GigyaLanguage { Language = "nl" });
                    context.Add(new GigyaLanguage { Language = "nl-inf" });
                    context.Add(new GigyaLanguage { Language = "fi" });
                    context.Add(new GigyaLanguage { Language = "fr" });
                    context.Add(new GigyaLanguage { Language = "fr-inf" });
                    context.Add(new GigyaLanguage { Language = "de" });
                    context.Add(new GigyaLanguage { Language = "de-inf" });
                    context.Add(new GigyaLanguage { Language = "el" });
                    context.Add(new GigyaLanguage { Language = "he" });
                    context.Add(new GigyaLanguage { Language = "hu" });
                    context.Add(new GigyaLanguage { Language = "id" });
                    context.Add(new GigyaLanguage { Language = "it" });
                    context.Add(new GigyaLanguage { Language = "ja" });
                    context.Add(new GigyaLanguage { Language = "ko" });
                    context.Add(new GigyaLanguage { Language = "ms" });
                    context.Add(new GigyaLanguage { Language = "no" });
                    context.Add(new GigyaLanguage { Language = "fa" });
                    context.Add(new GigyaLanguage { Language = "pl" });
                    context.Add(new GigyaLanguage { Language = "pt" });
                    context.Add(new GigyaLanguage { Language = "pt-br" });
                    context.Add(new GigyaLanguage { Language = "ro" });
                    context.Add(new GigyaLanguage { Language = "ru" });
                    context.Add(new GigyaLanguage { Language = "sr" });
                    context.Add(new GigyaLanguage { Language = "sk" });
                    context.Add(new GigyaLanguage { Language = "sl" });
                    context.Add(new GigyaLanguage { Language = "es" });
                    context.Add(new GigyaLanguage { Language = "es-inf" });
                    context.Add(new GigyaLanguage { Language = "es-mx" });
                    context.Add(new GigyaLanguage { Language = "sv" });
                    context.Add(new GigyaLanguage { Language = "tl" });
                    context.Add(new GigyaLanguage { Language = "th" });
                    context.Add(new GigyaLanguage { Language = "tr" });
                    context.Add(new GigyaLanguage { Language = "uk" });
                    context.Add(new GigyaLanguage { Language = "vi" });

                    context.SaveChanges();
                }

                _languages = context.Languages.ToDictionary(i => i.Language, j => true);
            }
        }
    }
}