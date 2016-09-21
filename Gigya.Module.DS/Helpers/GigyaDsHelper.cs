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
        //private readonly GigyaSettingsHelper _settingsHelper;
        private readonly IGigyaModuleSettings _settings;
        private readonly Logger _logger;
        private readonly GigyaDsSettingsHelper _dsSettingsHelper;
        private readonly GigyaDsApiHelper _apiHelper;

        public GigyaDsHelper(IGigyaModuleSettings settings, Logger logger, GigyaDsApiHelper apiHelper = null, GigyaDsSettingsHelper dsSettingsHelper = null)
        {
            //_settingsHelper = settingsHelper;
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

                // do some merge of the data
            }
            return model;
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
            return model.data;
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
            
            var model = new Dictionary<string, object>();

            foreach (var mappingType in settings.MappingsByType)
            {
                var dsType = mappingType.Key;
                foreach (var mapping in mappingType.Value.GroupBy(i => i.Custom.Oid))
                {
                    //var fieldName
                    var fields = mapping.Select(i => i.GigyaFieldName).Distinct();
                    var data = Get(uid, mapping.Key, dsType, fields);

                    if (data != null)
                    {
                        var dataDict = data as Dictionary<string, object>;
                        
                        // prob needs to be recursive and use method below:





                        //DynamicUtils.SetValue(model, string.Join(".", mappingType.Key, mapping.Key))
                        if (!model.ContainsKey(dsType))
                        {
                            model[dsType] = dataDict;
                        }
                        else
                        {
                            foreach (var pair in model.Concat(dataDict))
                            {
                                var typeProperties = model[dsType] as Dictionary<string, object>;
                                typeProperties[pair.Key] = pair.Value;

                            }
                        }
                    }
                }

                // do some merge of the data
            }

            return model.ToExpando();
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
            accountInfo.ds = DynamicUtils.GetValue<object>(data, "ds");
            return accountInfo;
        }
    }
}
