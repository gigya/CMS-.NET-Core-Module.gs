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
        private readonly Logger _logger;
        private static readonly string _configLocation = ConfigurationManager.AppSettings["Gigya.Ds.MappingFilePath"] ?? "~/App_Data/Gigya/dsMappings.json";
        private const string _cacheKey = "GigyaDsSettingsHelper-7F2E4041-553F-4FF4-9A3A-2BFBB1C05F17";

        public GigyaDsSettingsHelper(Logger logger)
        {
            _logger = logger;
        }

        public GigyaDsSettings Get()
        {
            var settings = MemoryCache.Default[_cacheKey] as GigyaDsSettings;
            if (settings != null)
            {
                return settings;
            }

            settings = Load();
            MemoryCache.Default[_cacheKey] = settings;
            return settings;
        }

        private GigyaDsSettings Load()
        {
            var filePath = _configLocation;
            if (filePath.StartsWith("~/"))
            {
                filePath = HostingEnvironment.MapPath(filePath);
            }

            if (!File.Exists(filePath))
            {
                _logger.Error("Gigya DS config file does not exist.");
                return null;
            }

            try
            {
                var jsonRaw = File.ReadAllText(filePath);

                var jsonSettings = new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                var model = JsonConvert.DeserializeObject<GigyaDsSettings>(jsonRaw, jsonSettings);

                foreach (var mapping in model.Mappings)
                {
                    var split = mapping.GigyaName.Split(new char[] { '.' }, 3);
                    if (split.Length == 3)
                    {
                        mapping.GigyaDsType = split[1];
                        mapping.GigyaFieldName = split[2];
                    }
                }

                model.MappingsByType = model.Mappings.GroupBy(i => i.GigyaDsType).ToDictionary(i => i.Key, j => j.ToList());
                return model;
            }
            catch(Exception e)
            {
                _logger.Error("Failed to parse Gigya DS config file.", e);
                throw;
            }
        }
    }
}
