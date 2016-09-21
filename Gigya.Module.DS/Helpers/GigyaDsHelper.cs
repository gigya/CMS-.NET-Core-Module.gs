using Gigya.Module.Core.Connector.Common;
using Gigya.Module.Core.Connector.Helpers;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.Core.Data;
using Gigya.Module.DS.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.DS.Helpers
{
    public class GigyaDsHelper
    {
        private readonly IGigyaModuleSettings _settings;
        private readonly Logger _logger;
        private readonly GigyaDsSettingsHelper _dsSettingsHelper;
        private readonly GigyaDsApiHelper _apiHelper;

        public GigyaDsHelper(IGigyaModuleSettings settings, Logger logger, GigyaDsApiHelper apiHelper = null, GigyaDsSettingsHelper dsSettingsHelper = null)
        {
            _settings = settings;
            _logger = logger;
            _apiHelper = apiHelper ?? new GigyaDsApiHelper(_logger);
            _dsSettingsHelper = dsSettingsHelper ?? new GigyaDsSettingsHelper(_logger);
        }

        public dynamic GetOrSearch(string uid)
        {
            var settings = _dsSettingsHelper.Get();
            if (settings == null)
            {
                return null;
            }

            switch (settings.Method)
            {
                case GigyaDsMethod.Get:
                    return GetAll(uid, settings);
                case GigyaDsMethod.Search:
                default:
                    return settings.Mappings.Count == 1 ? GetAll(uid, settings) : SearchAll(uid, settings);
            }
        }

        public dynamic SearchAll(string uid, GigyaDsSettings settings)
        {
            if (settings == null)
            {
                return null;
            }

            var model = new ExpandoObject();

            foreach (var mappingType in settings.MappingsByType)
            {
                var fields = mappingType.Value.Select(i => i.GigyaFieldName).Distinct();
                var data = Search(uid, mappingType.Key, fields);
                if (data != null)
                {
                    // do some merge of the data
                    Dictionary<string, object> mappedDsResult = MapDataToDsType(mappingType.Key, data);
                    model = DynamicUtils.Merge(model, mappedDsResult);
                }
            }
            return model;
        }

        private static Dictionary<string, object> MapDataToDsType(string mappingType, dynamic data)
        {
            var mappingTypeResult = new Dictionary<string, object>();
            mappingTypeResult[mappingType] = data;

            var result = new Dictionary<string, object>();
            result["ds"] = mappingTypeResult;
            return result;
        }

        public dynamic Search(string uid, string dsType, IEnumerable<string> fields = null)
        {
            var query = GigyaDsSearchHelper.BuildQuery(uid, dsType, fields);
            var response = _apiHelper.Search(_settings, query);
            if (response == null)
            {
                return null;
            }

            dynamic model = JsonConvert.DeserializeObject<ExpandoObject>(response.GetResponseText());
            
            var modelDictionary = model as IDictionary<string, object>;
            if (!modelDictionary.ContainsKey("results"))
            {
                return null;
            }

            var mergedResults = new ExpandoObject();
            foreach (var result in model.results)
            {
                var resultDictionary = result as IDictionary<string, object>;
                if (resultDictionary.ContainsKey("data"))
                {
                    mergedResults = DynamicUtils.Merge(mergedResults, result.data);
                }
            }

            return mergedResults;
        }

        public dynamic Get(string uid, string oid, string dsType, IEnumerable<string> fields = null)
        {
            var response = _apiHelper.Get(_settings, uid, oid, dsType, fields);
            if (response == null)
            {
                return null;
            }

            dynamic model = JsonConvert.DeserializeObject<ExpandoObject>(response.GetResponseText());
            return model.data;
        }

        public dynamic GetAll(string uid, GigyaDsSettings settings)
        {
            if (settings == null)
            {
                return null;
            }

            var model = new ExpandoObject();

            foreach (var mappingType in settings.MappingsByType)
            {
                var dsType = mappingType.Key;
                foreach (var mapping in mappingType.Value.GroupBy(i => i.Custom.Oid))
                {
                    var fields = mapping.Select(i => i.GigyaFieldName).Distinct();
                    var data = Get(uid, mapping.Key, dsType, fields);

                    if (data != null)
                    {
                        Dictionary<string, object> mappedDsResult = MapDataToDsType(mappingType.Key, data);
                        model = DynamicUtils.Merge(model, mappedDsResult);
                    }
                }
            }

            return model;
        }

        public dynamic Merge(dynamic accountInfo)
        {
            // get ds data
            var data = GetOrSearch(accountInfo.UID);
            if (data == null)
            {
                return accountInfo;
            }

            // merge with accountInfo
            accountInfo = DynamicUtils.Merge(accountInfo, data);
            return accountInfo;
        }
    }
}
