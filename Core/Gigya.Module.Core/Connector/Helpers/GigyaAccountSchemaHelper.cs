using Gigya.Module.Core.Connector.Common;
using Gigya.Module.Core.Connector.Models;
using Gigya.Module.Core.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace Gigya.Module.Core.Connector.Helpers
{
    public class GigyaAccountSchemaHelper<T> where T : GigyaModuleSettings
    {
        protected readonly GigyaApiHelper<T> _apiHelper;
        protected readonly T _gigyaModuleSettings;
        protected readonly string _cacheKey;
        private int _cacheMins = 10;

        public GigyaAccountSchemaHelper(GigyaApiHelper<T> apiHelper, T settings)
        {
            _apiHelper = apiHelper;
            _gigyaModuleSettings = settings;
            _cacheKey = string.Concat("GigyaAccountSchemaHelper.GetAccountSchema.", settings.ApiKey);
        }

        public AccountSchemaModel GetAccountSchema()
        {
            var response = HttpContext.Current.Cache[_cacheKey] as AccountSchemaModel;
            if (response != null)
            {
                return response;
            }

            var schemaData = _apiHelper.GetAccountSchema(_gigyaModuleSettings);
            if (schemaData == null || schemaData.GetErrorCode() != 0)
            {
                return null;
            }

            response = new AccountSchemaModel { Properties = new List<AccountSchemaProperty>() };
            var model = JsonConvert.DeserializeObject<ExpandoObject>(schemaData.GetResponseText());

            AddProperties(ref response, model, "profileSchema.fields", "profile");
            AddProperties(ref response, model, "dataSchema.fields", "data");
            AddProperties(ref response, model, "subscriptionsSchema.fields", "subscriptions");
            AddProperties(ref response, model, "preferencesSchema.fields", "preferences");

            HttpContext.Current.Cache.Insert(_cacheKey, response, null, DateTime.UtcNow.AddMinutes(_cacheMins), Cache.NoSlidingExpiration);
            return response;
        }

        private void AddProperties(ref AccountSchemaModel results, ExpandoObject model, string propertyPath, string propertyPrefix)
        {
            var fields = DynamicUtils.GetValue<IDictionary<string, object>>(model, propertyPath);
            if (fields == null || !fields.Any())
            {
                return;
            }

            results.Properties.AddRange(fields.Keys.Select(i => new AccountSchemaProperty
            {
                Name = string.Join(".", propertyPrefix, i)
            }));
        }
    }
}
