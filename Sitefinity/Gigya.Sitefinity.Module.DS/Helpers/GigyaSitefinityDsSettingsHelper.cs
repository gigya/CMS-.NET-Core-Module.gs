using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.DS.Config;
using Gigya.Module.Connector;
using Gigya.Sitefinity.Module.DS.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using Telerik.Sitefinity.Services;

namespace Gigya.Sitefinity.Module.DS.Helpers
{
    public class GigyaSitefinityDsSettingsHelper
    {
        private readonly Logger _logger;
        private const string _cacheKey = "GigyaSitefinityDsSettingsHelper-72C18F5C-F311-42DB-89D0-11A8DB93C785";
        private static readonly int _cacheMins = Convert.ToInt32(ConfigurationManager.AppSettings["Gigya.DS.CacheMins"] ?? "60");

        public GigyaSitefinityDsSettingsHelper(Logger logger)
        {
            _logger = logger;
        }

        public virtual void ClearCache(Guid siteId)
        {
            var cacheKey = string.Concat(_cacheKey, "__" + siteId);
            MemoryCache.Default.Remove(cacheKey);
        }

        /// <summary>
        /// Gets the settings based on the homepage for the current Umbraco page. 
        /// This method will only work if called within the Umbraco pipeline e.g. it will fail for ajax requests.
        /// </summary>
        /// <returns></returns>
        public GigyaDsSettings GetForCurrentSite()
        {
            // get current page id
            var siteId = Guid.Empty;
            if (SystemManager.CurrentContext.IsMultisiteMode)
            {
                siteId = SystemManager.CurrentContext.CurrentSite.Id;
            }

            var model = Get(siteId);
            return model;
        }

        /// <summary>
        /// Gets the DS settings for site with id of <paramref name="siteId"/>
        /// </summary>
        /// <param name="siteId">The site id to get the settings for.</param>
        /// <param name="useCache">Whether to use the cache.</param>
        /// <returns></returns>
        public virtual GigyaDsSettings Get(Guid siteId, bool useCache = true)
        {
            var cacheKey = string.Concat(_cacheKey, "__" + siteId);
            var settingsContainer = useCache ? MemoryCache.Default[cacheKey] as GigyaDsSettingsContainer : null;
            if (settingsContainer == null)
            {
                settingsContainer = Load(siteId);
                if (useCache && _cacheMins > 0)
                {
                    if (settingsContainer == null)
                    {
                        return null;
                    }
                    MemoryCache.Default.Set(cacheKey, settingsContainer, DateTime.Now.AddMinutes(_cacheMins));
                }
            }

            if (settingsContainer == null || settingsContainer.Sites == null || !settingsContainer.Sites.Any())
            {
                return null;
            }

            // get settings for site if possible otherwise fallback to default
            var siteIdString = siteId.ToString();
            var emptyGuid = Guid.Empty.ToString();
            var settings = settingsContainer.Sites.FirstOrDefault(i => i.SiteId.Any(j => j == siteIdString)) ?? settingsContainer.Sites.FirstOrDefault(i => i.SiteId.Any(j => j == emptyGuid));
            return settings;
        }

        /// <summary>
        /// Gets the settings from the db and maps.
        /// </summary>
        /// <returns></returns>
        private GigyaDsSettingsContainer Load(Guid id)
        {
            using (var context = GigyaDSContext.Get())
            {
                var settings = context.Settings.Where(i => i.SiteId == id || i.SiteId == Guid.Empty).Select(Map).ToList();

                var model = new GigyaDsSettingsContainer
                {
                    Sites = settings
                };

                return model;
            }
        }

        /// <summary>
        /// Maps Sitefinity DS model to a core model.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public GigyaDsSettings Map(GigyaSitefinityModuleDsSettings settings)
        {
            var mappedSetting = new GigyaDsSettings
            {
                Method = settings.Method,
                SiteId = new string[] { settings.SiteId.ToString() },
                Mappings = settings.Mappings.Select(Map).ToList()
            };

            mappedSetting.MappingsByType = mappedSetting.Mappings
                .Where(i => !string.IsNullOrEmpty(i.GigyaDsType))
                .GroupBy(i => i.GigyaDsType)
                .ToDictionary(i => i.Key, j => j.ToList());
            return mappedSetting;
        }

        /// <summary>
        /// Maps Sitefinity DS mapping model to a core model.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public GigyaDsMapping Map(GigyaSitefinityDsMapping source)
        {
            var mapping = new GigyaDsMapping
            {
                CmsName = source.CmsName,
                Custom = new Custom { Oid = source.Oid },
                GigyaName = source.GigyaName
            };

            var split = source.GigyaName.Split(new char[] { '.' }, 3);
            if (split.Length == 3)
            {
                mapping.GigyaDsType = split[1];
                mapping.GigyaFieldName = split[2];
            }

            return mapping;
        }

        /// <summary>
        /// Deletes site specific DS settings (used if the site will now inherit from the global settings).
        /// </summary>
        /// <param name="siteId"></param>
        public void Delete(Guid siteId)
        {
            if (siteId == Guid.Empty)
            {
                return;
            }

            using (var context = GigyaDSContext.Get())
            {
                var settings = context.Settings.FirstOrDefault(i => i.SiteId == siteId);
                if (settings != null)
                {
                    context.Delete(settings);
                    context.SaveChanges();
                }
            }
        }
    }
}
