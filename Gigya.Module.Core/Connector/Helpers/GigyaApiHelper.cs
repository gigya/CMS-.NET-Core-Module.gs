using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Gigya.Socialize;
using Gigya.Socialize.SDK;
using Gigya.Module.Core.Data;
using Gigya.Module.Core.Connector.Logging;
using Newtonsoft.Json;
using System.Dynamic;
using Gigya.Module.Core.Connector.Common;

namespace Gigya.Module.Core.Connector.Helpers
{
    public class GigyaApiHelper
    {
        private readonly GigyaSettingsHelper _settingsHelper;
        private readonly Logger _logger;

        public GigyaApiHelper(GigyaSettingsHelper settingsHelper, Logger logger)
        {
            _settingsHelper = settingsHelper;
            _logger = logger;
        }

        private GSRequest NewRequest(IGigyaModuleSettings settings, string applicationSecret, string method)
        {
            return new GSRequest(settings.ApiKey, applicationSecret, method, null, true, settings.ApplicationKey);
        }

        private GSRequest NewRequest(IGigyaModuleSettings settings, string method)
        {
            return NewRequest(settings, settings.ApplicationSecret, method);
        }

        public GSResponse VerifySettings(IGigyaModuleSettings settings, string applicationSecret)
        {
            var method = "accounts.getPolicies";
            var request = NewRequest(settings, applicationSecret, method);
            var response = Send(request, method, settings);
            return response;
        }

        public GSResponse ExchangeSignature(string userId, IGigyaModuleSettings settings, string userIdSignature, string signatureTimestamp, string userKey)
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
        public bool ValidateApplicationKeySignature(string userId, IGigyaModuleSettings settings, GSResponse originalResponse)
        {
            // this uses the Send method which validates the signature
            var response = ExchangeSignature(userId, settings, originalResponse.GetString(Constants.GigyaFields.UserIdSignature, null),
                originalResponse.GetString(Constants.GigyaFields.SignatureTimestamp, null), settings.ApplicationKey);
            
            return ValidateExchangeSignatureResponse(response);
        }

        private bool ValidateExchangeSignatureResponse(GSResponse response)
        {
            // if signature is invalid a null response is returned
            return response != null && response.GetErrorCode() == 0 && !string.IsNullOrEmpty(response.GetString(Constants.GigyaFields.UserId, null));
        }

        /// <summary>
        /// Validates an application key signatures by calling accounts.exchangeUIDSignature and checking the response signature.
        /// </summary>
        public bool ValidateApplicationKeySignature(string userId, IGigyaModuleSettings settings, string signatureTimestamp, string signature)
        {
            var response = ExchangeSignature(userId, settings, signature, signatureTimestamp, settings.ApplicationKey);
            return ValidateExchangeSignatureResponse(response);
        }

        public GSResponse GetAccountInfo(string userId, IGigyaModuleSettings settings)
        {
            var method = "accounts.getAccountInfo";
            var request = NewRequest(settings, method);
            request.SetParam("UID", userId);
            request.SetParam("include", "identities-active,identities-all,loginIDs,emails,profile,data,password,lastLoginLocation,regSource,irank");

            var response = Send(request, method, settings);
            return response;
        }

        public bool ValidateSignature(string userId, IGigyaModuleSettings settings, GSResponse response, bool disableSignatureExchange = false)
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
        private GSResponse Send(GSRequest request, string apiMethod, IGigyaModuleSettings settings, bool disableSignatureExchange = false)
        {
            if (apiMethod == "accounts.getAccountInfo")
            {
                var environment = new
                {
                    cms_name = _settingsHelper.CmsName,
                    cms_version = _settingsHelper.CmsVersion,
                    gigya_version = _settingsHelper.ModuleVersion
                };

                request.SetParam("environment", JsonConvert.SerializeObject(environment));
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
                var gigyaErrorDetail = DynamicUtils.GetValue<string>(gigyaModel, "errorDetails");
                var gigyaCallId = DynamicUtils.GetValue<string>(gigyaModel, "callId");

                _logger.Error(string.Format("API call: {0}. CallId: {1}. Error: {2}. Error Details: {3}.", apiMethod, gigyaCallId, gigyaError, gigyaErrorDetail), e);
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

                    _logger.DebugFormat("Invalid user signature for login request. API call: {0}. CallId: {1}.", apiMethod, gigyaCallId);
                }
                return null;
            }

            return response;
        }

        private void LogRequestIfRequired(IGigyaModuleSettings settings, string apiMethod)
        {
            if (settings.DebugMode)
            {
                _logger.DebugFormat("Request API call: {0}.", apiMethod);
            }
        }

        private void LogResponseIfRequired(IGigyaModuleSettings settings, string apiMethod, GSResponse response)
        {
            if (settings.DebugMode)
            {
                dynamic gigyaModel = response != null ? JsonConvert.DeserializeObject<ExpandoObject>(response.GetResponseText()) : new ExpandoObject();
                var gigyaError = response != null ? response.GetErrorMessage() : string.Empty;
                var gigyaErrorDetail = DynamicUtils.GetValue<string>(gigyaModel, "errorDetails");

                var callId = DynamicUtils.GetValue<string>(gigyaModel, "callId");
                _logger.DebugFormat("API call: {0}. CallId: {1}. Error: {2}. Error Details: {3}.", apiMethod, callId, gigyaError, gigyaErrorDetail);
            }
        }
    }
}
