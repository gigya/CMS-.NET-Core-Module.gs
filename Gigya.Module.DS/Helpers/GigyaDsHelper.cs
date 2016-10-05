using Gigya.Module.Core.Connector.Common;
using Gigya.Module.Core.Connector.Helpers;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.Core.Connector.Models;
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
        private readonly GigyaDsSettings _dsSettings;
        private readonly GigyaDsApiHelper _apiHelper;

        public GigyaDsHelper(IGigyaModuleSettings settings, Logger logger, GigyaDsSettings dsSettings, GigyaDsApiHelper apiHelper = null)
        {
            _settings = settings;
            _logger = logger;
            _apiHelper = apiHelper ?? new GigyaDsApiHelper(_logger);
            _dsSettings = dsSettings;
        }

        /// <summary>
        /// Add the ds mappings to the main module field mappings. 
        /// This means they will be picked up when doing the normal getAccountInfo field mapping.
        /// </summary>
        /// <param name="mappingFields">The current module field mappings.</param>
        /// <param name="dsSettings"></param>
        public void AddMappingFields(List<MappingField> mappingFields)
        {
            if (_dsSettings == null)
            {
                return;
            }
            
            foreach (var mapping in _dsSettings.Mappings)
            {
                if (!mappingFields.Any(i => i.CmsFieldName == mapping.CmsName))
                {
                    mappingFields.Add(new MappingField
                    {
                        CmsFieldName = mapping.CmsName,
                        GigyaFieldName = mapping.GigyaName
                    });
                }
            }
        }

        /// <summary>
        /// Fetches ds data using the configured method (get or search).
        /// </summary>
        /// <param name="uid">The user Id.</param>
        public dynamic GetOrSearch(string uid)
        {
            if (_dsSettings == null)
            {
                return null;
            }

            switch (_dsSettings.Method)
            {
                case GigyaDsMethod.Get:
                    return GetAll(uid);
                case GigyaDsMethod.Search:
                default:
                    return _dsSettings.Mappings.Count == 1 ? GetAll(uid) : SearchAll(uid);
            }
        }

        public string Validate()
        {
            foreach (var mappingType in _dsSettings.MappingsByType)
            {
                var fields = mappingType.Value.Select(i => i.GigyaFieldName).Distinct();
                dynamic data = GetSchema(mappingType.Key);

                if (data == null)
                {
                    return string.Format("ds type [{0}] does not exist.", mappingType.Key);
                }

                foreach (var field in fields)
                {
                    if (DynamicUtils.GetValue<object>(data, string.Concat("schema.fields.", field)) == null)
                    {
                        return string.Format("ds.{0}.{1} does not exist.", mappingType.Key, field);
                    }
                }
            }

            return null;
        }

        private ExpandoObject GetSchema(string dsType)
        {
            var response = _apiHelper.GetSchema(_settings, dsType);
            if (response == null)
            {
                return null;
            }

            var model = JsonConvert.DeserializeObject<ExpandoObject>(response.GetResponseText());
            return model;
        }

        /// <summary>
        /// Fetches all ds data using ds.search
        /// </summary>
        /// <param name="uid">The user Id.</param>
        public ExpandoObject SearchAll(string uid)
        {
            if (_dsSettings == null)
            {
                return null;
            }

            var model = new Dictionary<string, object>();

            foreach (var mappingType in _dsSettings.MappingsByType)
            {
                var fields = mappingType.Value.Select(i => i.GigyaFieldName).Distinct();
                var data = Search(uid, mappingType.Key, fields);
                if (data != null)
                {
                    // do some merge of the data
                    Dictionary<string, object> mappedDsResult = MapDataToDsType(mappingType.Key, data);
                    model = DynamicUtils.MergeToDictionary(model, mappedDsResult);
                }
            }
            return model.ToExpando();
        }

        /// <summary>
        /// Prefixes ds data so it's in a ds.[type].[data] object.
        /// </summary>
        /// <param name="mappingType">The type param used in the ds API.</param>
        /// <param name="data">The ds data returned from Gigya.</param>
        /// <returns></returns>
        private static Dictionary<string, object> MapDataToDsType(string mappingType, dynamic data)
        {
            var mappingTypeResult = new Dictionary<string, object>();
            mappingTypeResult[mappingType] = data;

            var result = new Dictionary<string, object>();
            result["ds"] = mappingTypeResult;
            return result;
        }

        /// <summary>
        /// Uses the Gigya ds.search API call to retrieve data.
        /// </summary>
        /// <param name="uid">The user Id.</param>
        /// <param name="dsType">The ds type that is passed in the type param.</param>
        /// <param name="fields">A collection of field names or null to have them created from the settings.</param>
        /// <returns></returns>
        public dynamic Search(string uid, string dsType, IEnumerable<string> fields = null)
        {
            if (string.IsNullOrEmpty(uid))
            {
                throw new ArgumentException("uid");
            }

            if (string.IsNullOrEmpty(dsType))
            {
                throw new ArgumentException("dsType");
            }

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

        /// <summary>
        /// Gets a single result from the ds using ds.get.
        /// </summary>
        /// <param name="oid">The ds OID.</param>
        /// <param name="uid">The user Id.</param>
        /// <param name="dsType">The ds type that is passed in the type param.</param>
        /// <param name="fields">A collection of field names or null to have them created from the settings.</param>
        /// <returns></returns>
        public dynamic Get(string uid, string oid, string dsType, IEnumerable<string> fields = null)
        {
            if (string.IsNullOrEmpty(uid))
            {
                throw new ArgumentException("uid");
            }

            if (string.IsNullOrEmpty(oid))
            {
                throw new ArgumentException("oid");
            }

            if (string.IsNullOrEmpty(dsType))
            {
                throw new ArgumentException("dsType");
            }

            var response = _apiHelper.Get(_settings, uid, oid, dsType, fields);
            if (response == null)
            {
                return null;
            }

            dynamic model = JsonConvert.DeserializeObject<ExpandoObject>(response.GetResponseText());

            // check data property exists
            var resultDictionary = model as IDictionary<string, object>;
            if (!resultDictionary.ContainsKey("data"))
            {
                return null;
            }

            return model.data;
        }

        /// <summary>
        /// Fetches ds data using ds.get
        /// </summary>
        /// <param name="uid">The user Id.</param>
        /// <param name="settings">The mapping settings.</param>
        /// <returns></returns>
        public ExpandoObject GetAll(string uid)
        {
            if (_dsSettings == null)
            {
                return null;
            }

            var model = new Dictionary<string, object>();

            foreach (var mappingType in _dsSettings.MappingsByType)
            {
                var dsType = mappingType.Key;
                foreach (var mapping in mappingType.Value.GroupBy(i => i.Custom.Oid))
                {
                    var fields = mapping.Select(i => i.GigyaFieldName).Distinct();
                    var data = Get(uid, mapping.Key, dsType, fields);

                    if (data != null)
                    {
                        Dictionary<string, object> mappedDsResult = MapDataToDsType(mappingType.Key, data);
                        model = DynamicUtils.MergeToDictionary(model, mappedDsResult);
                    }
                }
            }

            return model.ToExpando();
        }

        /// <summary>
        /// Merges the getAccountInfo model with the ds data and adds the DS mapping fields to <param name="mappingFields"></param>
        /// </summary>
        /// <param name="accountInfo"></param>
        /// <returns></returns>
        public dynamic Merge(dynamic accountInfo, List<MappingField> mappingFields)
        {
            // get ds data
            var data = GetOrSearch(accountInfo.UID);
            if (data == null)
            {
                return accountInfo;
            }

            // merge with accountInfo
            accountInfo = DynamicUtils.Merge(accountInfo, data);
            AddMappingFields(mappingFields);
            return accountInfo;
        }
    }
}
