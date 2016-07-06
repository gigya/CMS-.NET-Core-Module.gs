using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Gigya.Socialize;
using Gigya.Socialize.SDK;
using Gigya.Module.Data;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Configuration;
using Gigya.Module.Connector.Logging;
using Newtonsoft.Json;
using System.Dynamic;
using Gigya.Module.Connector.Common;

namespace Gigya.Module.Connector.Helpers
{
    public class GigyaApiHelper
    {
        private static GSRequest NewRequest(GigyaModuleSettings settings, string applicationSecret, string method)
        {
            return new GSRequest(settings.ApiKey, applicationSecret, method, null, true, settings.ApplicationKey);
        }

        private static GSRequest NewRequest(GigyaModuleSettings settings, string method)
        {
            return NewRequest(settings, settings.ApplicationSecret, method);
        }

        public static GSResponse VerifySettings(Guid siteId)
        {
            var settings = GigyaSettingsHelper.Get(siteId, true);
            return VerifySettings(settings, settings.ApplicationSecret);
        }

        public static GSResponse VerifySettings(GigyaModuleSettings settings, string applicationSecret)
        {
            var method = "socialize.shortenURL";
            var request = NewRequest(settings, applicationSecret, method);
            request.SetParam("url", "http://gigya.com");

            var response = Send(request, method, settings);
            return response;
        }

        public static GSResponse ExchangeSignature(string userId, GigyaModuleSettings settings, string userIdSignature, string signatureTimestamp, string userKey)
        {
            var method = "accounts.exchangeUIDSignature";
            var request = NewRequest(settings, method);
            request.SetParam("UID", userId);
            request.SetParam("UIDSignature", userIdSignature);
            request.SetParam("signatureTimestamp", signatureTimestamp);
            request.SetParam("userKey", userKey);

            var response = Send(request, method, settings, true);
            return response;
        }

        /// <summary>
        /// Validates an application key signatures by calling accounts.exchangeUIDSignature and checking the response signature.
        /// </summary>
        public static bool ValidateApplicationKeySignature(string userId, GigyaModuleSettings settings, GSResponse originalResponse)
        {
            // this uses the Send method which validates the signature
            var response = ExchangeSignature(userId, settings, originalResponse.GetString(Constants.GigyaFields.UserIdSignature, null),
                originalResponse.GetString(Constants.GigyaFields.SignatureTimestamp, null), settings.ApplicationKey);
            
            return ValidateExchangeSignatureResponse(response);
        }

        private static bool ValidateExchangeSignatureResponse(GSResponse response)
        {
            // if signature is invalid a null response is returned
            return response != null && response.GetErrorCode() == 0 && !string.IsNullOrEmpty(response.GetString(Constants.GigyaFields.UserId, null));
        }

        /// <summary>
        /// Validates an application key signatures by calling accounts.exchangeUIDSignature and checking the response signature.
        /// </summary>
        public static bool ValidateApplicationKeySignature(string userId, GigyaModuleSettings settings, string signatureTimestamp, string signature)
        {
            var response = ExchangeSignature(userId, settings, signature, signatureTimestamp, settings.ApplicationKey);
            return ValidateExchangeSignatureResponse(response);
        }

        public static GSResponse GetAccountInfo(string userId, GigyaModuleSettings settings)
        {
            var method = "accounts.getAccountInfo";
            var request = NewRequest(settings, method);
            request.SetParam("UID", userId);
            request.SetParam("include", "identities-active,identities-all,loginIDs,emails,profile,data,password,lastLoginLocation,regSource,irank");

            var response = Send(request, method, settings);
            return response;
        }

        public static bool ValidateSignature(string userId, GigyaModuleSettings settings, GSResponse response, bool disableSignatureExchange = false)
        {
            if (disableSignatureExchange)
            {
                return SigUtils.ValidateUserSignature(userId, response.GetString(Constants.GigyaFields.SignatureTimestamp, null),
                    settings.ApplicationSecret, response.GetString(Constants.GigyaFields.UserIdSignature, null));
            }

            return ValidateApplicationKeySignature(userId, settings, response);
        }

        /// <summary>
        /// Sends a request to the Gigya API.
        /// </summary>
        /// <param name="request">Request object.</param>
        /// <param name="apiMethod">The Gigya method to call.</param>
        /// <param name="settings">The Gigya module settings.</param>
        /// <param name="disableSignatureExchange">If set to true, the signature won't be exchanged even if there's an application key set in the settings.</param>
        /// <returns></returns>
        private static GSResponse Send(GSRequest request, string apiMethod, GigyaModuleSettings settings, bool disableSignatureExchange = false)
        {
            if (apiMethod == "accounts.getAccountInfo")
            {
                request.SetParam("gigya_version", ModuleClass.Version);
                request.SetParam("cms_version", GigyaSettingsHelper.SitefinityVersion);
            }

            if (!string.IsNullOrEmpty(settings.DataCenter))
            {
                request.APIDomain = settings.DataCenter + ".gigya.com";
            }

            LogRequestIfRequired(settings, apiMethod);

            GSResponse response = null;

            try
            {
                response = request.Send();
            }
            catch (Exception e)
            {
                dynamic gigyaModel = response != null ? JsonConvert.DeserializeObject<ExpandoObject>(response.GetResponseText()) : new ExpandoObject();
                var gigyaError = response != null ? response.GetErrorMessage() : string.Empty;
                var gigyaCallId = DynamicUtils.GetValue<string>(gigyaModel, "callId");

                Logger.Error(string.Format("API call: {0}. CallId: {1}. Error: {2}.", apiMethod, gigyaCallId, gigyaError), e);
                return response;
            }

            LogResponseIfRequired(settings, apiMethod, response);

            if (response.GetErrorCode() != 0)
            {
                return response;
            }

            var userId = response.GetString(Constants.GigyaFields.UserId, null);
            if (string.IsNullOrEmpty(userId))
            {
                return response;
            }
            
            if (!ValidateSignature(userId, settings, response, disableSignatureExchange))
            {
                if (settings.DebugMode)
                {
                    dynamic gigyaModel = response != null ? JsonConvert.DeserializeObject<ExpandoObject>(response.GetResponseText()) : new ExpandoObject();
                    var gigyaCallId = DynamicUtils.GetValue<string>(gigyaModel, "callId");

                    Logger.DebugFormat("Invalid user signature for login request. API call: {0}. CallId: {1}.", apiMethod, gigyaCallId);
                }
                return null;
            }

            return response;
        }

        private static void LogRequestIfRequired(GigyaModuleSettings settings, string apiMethod)
        {
            if (settings.DebugMode)
            {
                Logger.DebugFormat("Request API call: {0}.", apiMethod);
            }
        }

        private static void LogResponseIfRequired(GigyaModuleSettings settings, string apiMethod, GSResponse response)
        {
            if (settings.DebugMode)
            {
                dynamic gigyaModel = response != null ? JsonConvert.DeserializeObject<ExpandoObject>(response.GetResponseText()) : new ExpandoObject();

                // remove PII data (profile and data)
                gigyaModel.profile = null;
                gigyaModel.data = null;

                var callId = DynamicUtils.GetValue<string>(gigyaModel, "callId");
                Logger.DebugFormat("Response from API call: {0}. CallId: {1}. Response: {2}.", apiMethod, callId, JsonConvert.SerializeObject(gigyaModel));
            }
        }
    }
}
