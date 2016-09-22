using Gigya.Module.Core.Connector.Common;
using Gigya.Module.Core.Connector.Helpers;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.Core.Data;
using Gigya.Socialize.SDK;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.DS.Helpers
{
    public class GigyaDsApiHelper
    {
        private readonly Logger _logger;

        public GigyaDsApiHelper(Logger logger)
        {
            _logger = logger;
        }

        protected virtual GSRequest NewRequest(IGigyaModuleSettings settings, string applicationSecret, string method)
        {
            // real
            var request = new GSRequest(settings.ApiKey, applicationSecret, method, null, true, settings.ApplicationKey);

            if (settings.ApiKey == "3_qkAT5OcGyvYpkjc_VF6-OfoeTKGk4T_jVwjFF9f5TQzoAg-mH8SBsjQi1srdsOm6")
            {
                // hack
                applicationSecret = "bESFXHevdLlCtAMBxMyjnfmTdERrKyQOM919miLuUAw=";
                string apiKey = "3_rUQPSDMvtjnxvxk5wMrH-rnlNdwdCiwHRms4Ep3-JooNXlS9xYP-zPOm7zhZoeZ0";
                request = new GSRequest(apiKey, applicationSecret, method, null, true);
            }
            
            if (!string.IsNullOrEmpty(settings.DataCenter))
            {
                request.APIDomain = settings.DataCenter + ".gigya.com";
            }

            return request;
        }

        /// <summary>
        /// Calls ds.get to get ds data.
        /// </summary>
        /// <param name="settings">Current module settings.</param>
        /// <param name="uid">The user Id.</param>
        /// <param name="oid">The oid of the ds field.</param>
        /// <param name="gigyaType">The ds type.</param>
        /// <param name="fields">The fields to retrieve.</param>
        public GSResponse Get(IGigyaModuleSettings settings, string uid, string oid, string gigyaType, IEnumerable<string> fields = null)
        {
            var request = NewRequest(settings, settings.ApplicationSecret, "ds.get");
            request.SetParam("UID", uid);
            request.SetParam("oid", oid);
            request.SetParam("type", gigyaType);

            if (fields != null && fields.Any())
            {
                var fieldsValue = string.Join(",", fields);
                request.SetParam("fields", fieldsValue);
            }

            var response = Send(request, "ds.get", settings);
            return response;
        }

        /// <summary>
        /// Runs a ds.search query to retrieve ds data.
        /// </summary>
        /// <param name="settings">Current module settings.</param>
        /// <param name="query">The search query.</param>
        public GSResponse Search(IGigyaModuleSettings settings, string query)
        {
            var request = NewRequest(settings, settings.ApplicationSecret, "ds.search");
            request.SetParam("query", query);
            var response = Send(request, "ds.search", settings);
            return response;
        }

        private GSResponse Send(GSRequest request, string apiMethod, IGigyaModuleSettings settings)
        {
            GSResponse response = null;

            try
            {
                response = request.Send();
            }
            catch (Exception e)
            {
                return LogError(response, e);
            }

            GigyaApiHelper.LogResponseIfRequired(_logger, settings, apiMethod, response);

            if (response.GetErrorCode() != 0)
            {
                LogError(response, null);
                return null;
            }

            return response;
        }

        private GSResponse LogError(GSResponse response, Exception e)
        {
            dynamic gigyaModel = response != null ? JsonConvert.DeserializeObject<ExpandoObject>(response.GetResponseText()) : new ExpandoObject();
            var gigyaError = response != null ? response.GetErrorMessage() : string.Empty;
            var gigyaErrorDetail = DynamicUtils.GetValue<string>(gigyaModel, "errorDetails");
            var gigyaCallId = DynamicUtils.GetValue<string>(gigyaModel, "callId");

            _logger.Error(string.Format("API call: {0}. CallId: {1}. Error: {2}. Error Details: {3}.", "ds.get", gigyaCallId, gigyaError, gigyaErrorDetail), e);
            return response;
        }
    }
}
