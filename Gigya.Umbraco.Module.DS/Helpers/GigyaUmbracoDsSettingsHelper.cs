using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.DS.Config;
using Gigya.Umbraco.Module.Connector;
using Gigya.Umbraco.Module.DS.Data;
using Gigya.Umbraco.Module.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web;

namespace Gigya.Umbraco.Module.DS.Helpers
{
    public class GigyaUmbracoDsSettingsHelper
    {
        private readonly Logger _logger;
        private const string _cacheKey = "GigyaUmbracoDsSettingsHelper-7F2E4041-553F-4FF4-9A3A-2BFBB1C05F17";
        private static readonly int _cacheMins = Convert.ToInt32(ConfigurationManager.AppSettings["Gigya.DS.CacheMins"] ?? "60");

        public GigyaUmbracoDsSettingsHelper(Logger logger)
        {
            _logger = logger;
        }

        public virtual void ClearCache(int siteId)
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
            var homepageId = GigyaSettingsHelper.CurrentHomepageId();
            var model = Get(homepageId);
            return model;
        }

        public virtual GigyaDsSettings Get(int siteId, bool useCache = true)
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
            var settings = settingsContainer.Sites.FirstOrDefault(i => i.SiteId.Any(j => j == siteIdString)) ?? settingsContainer.Sites.FirstOrDefault(i => i.SiteId.Any(j => j == "-1"));
            return settings;
        }

        /// <summary>
        /// Gets the settings from the db and maps.
        /// </summary>
        /// <returns></returns>
        private GigyaDsSettingsContainer Load(int id)
        {
            var db = UmbracoContext.Current.Application.DatabaseContext.Database;

            var results = db.Fetch<GigyaUmbracoModuleDsSettings>(string.Format("SELECT * FROM gigya_ds_settings WHERE Id IN (-1, {0})", id));
            if (!results.Any())
            {
                return null;
            }

            var mappings = db.Fetch<GigyaUmbracoDsMapping>(string.Format("SELECT * FROM gigya_ds_mapping WHERE DsSettingId IN (-1, {0})", id));

            var model = new GigyaDsSettingsContainer
            {
                Sites = new List<GigyaDsSettings>(results.Count)
            };

            foreach (var result in results)
            {
                GigyaDsSettings mappedSetting = Map(mappings, result);

                model.Sites.Add(mappedSetting);
            }

            return model;
        }

        public GigyaDsSettings Map(List<GigyaUmbracoDsMapping> mappings, GigyaUmbracoModuleDsSettings settings)
        {
            var mappedSetting = new GigyaDsSettings
            {
                Method = (GigyaDsMethod)settings.Method,
                SiteId = new string[] { settings.Id.ToString() },
                Mappings = mappings.Where(i => i.DsSettingId == settings.Id).Select(Map).ToList()
            };

            mappedSetting.MappingsByType = mappedSetting.Mappings
                .Where(i => !string.IsNullOrEmpty(i.GigyaDsType))
                .GroupBy(i => i.GigyaDsType)
                .ToDictionary(i => i.Key, j => j.ToList());
            return mappedSetting;
        }

        public GigyaDsMapping Map(GigyaUmbracoDsMapping source)
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

        public GigyaUmbracoModuleDsSettings GetRaw(int id)
        {
            var db = UmbracoContext.Current.Application.DatabaseContext.Database;
            return db.SingleOrDefault<GigyaUmbracoModuleDsSettings>(id) ?? new GigyaUmbracoModuleDsSettings { Id = id, IsNew = true };
        }

        public void Save(GigyaUmbracoModuleDsSettings settings)
        {
            var db = UmbracoContext.Current.Application.DatabaseContext.Database;

            if (settings.IsNew)
            {
                db.BeginTransaction();
                db.Insert(settings);

                foreach (var mapping in settings.Mappings)
                {
                    db.Insert(mapping);
                }
                db.CompleteTransaction();
            }
            else
            {
                db.BeginTransaction();

                // clear old ones
                db.ExecuteScalar<GigyaUmbracoDsMapping>("DELETE FROM gigya_ds_mapping WHERE DsSettingId = " + settings.Id);

                foreach (var mapping in settings.Mappings)
                {
                    db.Insert(mapping);
                }
                db.Save(settings);
                db.CompleteTransaction();
            }
        }

        public void Delete(GigyaUmbracoModuleDsSettings settings)
        {
            if (settings.Id == -1)
            {
                return;
            }

            var db = UmbracoContext.Current.Application.DatabaseContext.Database;

            db.BeginTransaction();

            db.ExecuteScalar<GigyaUmbracoDsMapping>("DELETE FROM gigya_ds_mapping WHERE DsSettingId = " + settings.Id);

            db.Delete(settings);
            db.CompleteTransaction();
        }
    }
}
