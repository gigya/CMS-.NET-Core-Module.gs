using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.DS.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Gigya.Module.DS.Helpers
{
    public class GigyaDsSettingsHelper
    {
        private static readonly object _padlock = new object();
        private static FileSystemWatcher _watcher;
        private readonly Logger _logger;
        private static readonly string _configLocation = ConfigurationManager.AppSettings["Gigya.Ds.MappingFilePath"] ?? "~/App_Data/Gigya/dsMappings.json";
        private const string _cacheKey = "GigyaDsSettingsHelper-7F2E4041-553F-4FF4-9A3A-2BFBB1C05F17";

        public GigyaDsSettingsHelper(Logger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets the ds settings.
        /// </summary>
        /// <param name="siteId">Id of the current site.</param>
        /// <returns></returns>
        public virtual GigyaDsSettings Get(string siteId)
        {
            var settingsContainer = MemoryCache.Default[_cacheKey] as GigyaDsSettingsContainer;
            if (settingsContainer == null)
            {
                settingsContainer = Load();
                if (settingsContainer == null)
                {
                    return null;
                }
                MemoryCache.Default.Set(_cacheKey, settingsContainer, DateTime.Now.AddMinutes(60));
            }

            if (settingsContainer.Sites == null || !settingsContainer.Sites.Any())
            {
                return null;
            }

            // get settings for site if possible otherwise fallback to default
            var settings = settingsContainer.Sites.FirstOrDefault(i => i.SiteId.Any(j => j == siteId)) ?? settingsContainer.Sites.FirstOrDefault(i => i.SiteId.Any(j => j == "-1"));
            return settings;
        }

        /// <summary>
        /// Parses the ds settings json file and adds a file watch to be notified of updates.
        /// </summary>
        /// <returns></returns>
        private GigyaDsSettingsContainer Load()
        {
            var filePath = _configLocation;
            if (!string.IsNullOrEmpty(filePath) && filePath.StartsWith("~/"))
            {
                filePath = HostingEnvironment.MapPath(filePath);
            }

            if (!File.Exists(filePath))
            {
                _logger.Error(string.Concat("Data Storage JSON file wasn't found. Tried to use: ", filePath));
                return null;
            }

            try
            {
                AddFileWatcher(filePath);

                var jsonRaw = File.ReadAllText(filePath);

                var jsonSettings = new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                var model = JsonConvert.DeserializeObject<GigyaDsSettingsContainer>(jsonRaw, jsonSettings);

                foreach (var site in model.Sites)
                {
                    foreach (var mapping in site.Mappings)
                    {
                        var split = mapping.GigyaName.Split(new char[] { '.' }, 3);
                        if (split.Length == 3)
                        {
                            mapping.GigyaDsType = split[1];
                            mapping.GigyaFieldName = split[2];
                        }
                    }

                    site.MappingsByType = site.Mappings.Where(i => !string.IsNullOrEmpty(i.GigyaDsType)).GroupBy(i => i.GigyaDsType).ToDictionary(i => i.Key, j => j.ToList());
                }
                
                return model;
            }
            catch (JsonReaderException e)
            {
                _logger.Error("Invalid Gigya Data Storage mapping JSON structure.", e);
            }
            catch(Exception e)
            {
                _logger.Error("Failed to parse Gigya DS config file.", e);
            }

            return new GigyaDsSettingsContainer();
        }

        /// <summary>
        /// Creates a watcher so that updates to the mappings json file clears the cache.
        /// </summary>
        /// <param name="path"></param>
        private static void AddFileWatcher(string path)
        {
            if (_watcher != null)
            {
                return;
            }

            lock (_padlock)
            {
                if(_watcher != null)
                {
                    return;
                }

                var dir = Path.GetDirectoryName(path);
                var filename = Path.GetFileName(path);
                _watcher = new FileSystemWatcher(dir, filename);
                _watcher.NotifyFilter = NotifyFilters.LastWrite;
                _watcher.EnableRaisingEvents = true;
                _watcher.Changed += Watcher_Changed;
            }
        }

        private static void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            MemoryCache.Default.Remove(_cacheKey);
        }
    }
}
