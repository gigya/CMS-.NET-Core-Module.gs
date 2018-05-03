using Gigya.Module.Core.Connector.Common;
using Gigya.Module.Core.Connector.Events;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.Core.Connector.Models;
using Gigya.Module.Core.Data;
using Gigya.Module.Core.Mvc.Models;
using Gigya.Socialize.SDK;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Gigya.Module.Core.Connector.Helpers
{
    public abstract class GigyaMembershipHelperBase
    {
        protected readonly GigyaAccountHelperBase _gigyaAccountHelper;
        protected readonly GigyaApiHelper _gigyaApiHelper;
        protected readonly Logger _logger;

        public GigyaMembershipHelperBase(GigyaApiHelper apiHelper, GigyaAccountHelperBase gigyaAccountHelper, Logger logger)
        {
            _gigyaApiHelper = apiHelper;
            _logger = logger;
            _gigyaAccountHelper = gigyaAccountHelper;
        }

        protected abstract string CmsUserIdField { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gigyaModel"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected virtual string GetGigyaValueWithDefault(dynamic gigyaModel, string key, string defaultValue = null)
        {
            return DynamicUtils.GetValue<string>(gigyaModel, key) ?? defaultValue;
        }

        protected string GetGigyaFieldFromCmsAlias(dynamic gigyaModel, string cmsFieldName, string fallback, List<MappingField> mappingFields)
        {
            if (mappingFields == null)
            {
                return fallback;
            }

            var field = mappingFields.FirstOrDefault(i => i.CmsFieldName == cmsFieldName);
            if (field != null && !string.IsNullOrEmpty(field.GigyaFieldName))
            {
                return GetGigyaValueWithDefault(gigyaModel, field.GigyaFieldName, fallback);
            }

            return fallback;
        }

        protected string GetMappedFieldWithFallback(dynamic gigyaModel, string cmsFieldName, string gigyaFallbackFieldName, List<MappingField> mappingFields)
        {
            var value = GetGigyaFieldFromCmsAlias(gigyaModel, cmsFieldName, null, mappingFields);
            if (string.IsNullOrEmpty(value))
            {
                // no mapping provided for field so use the default
                value = GetGigyaValueWithDefault(gigyaModel, gigyaFallbackFieldName, null);
            }

            return value;
        }

        protected bool PersistentAuthRequired(IGigyaModuleSettings settings)
        {
            if (settings.SessionProvider == Enums.GigyaSessionProvider.CMS || settings.SessionProvider != Enums.GigyaSessionProvider.Gigya)
            {
                // legacy behaviour
                return false;
            }

            switch (settings.GigyaSessionMode)
            {
                case Enums.GigyaSessionMode.Session:
                    return false;
                default:
                    return true;
            }
        }

        protected GSResponse ValidateRequest(LoginModel model, IGigyaModuleSettings settings)
        {
            if (!settings.EnableRaas)
            {
                if (settings.DebugMode)
                {
                    _logger.Debug("RaaS not enabled so login aborted.");
                }
                return null;
            }

            if (!_gigyaApiHelper.ValidateApplicationKeySignature(model.UserId, settings, model.SignatureTimestamp, model.Signature))
            {
                if (settings.DebugMode)
                {
                    _logger.Debug("Invalid user signature for login request.");
                }
                return null;
            }

            var userInfoResponse = _gigyaApiHelper.GetAccountInfo(model.UserId, settings);
            if (userInfoResponse == null || userInfoResponse.GetErrorCode() != 0)
            {
                if (settings.DebugMode)
                {
                    _logger.Error("Failed to getAccountInfo");
                }
                return null;
            }

            return userInfoResponse;
        }

        protected List<MappingField> GetMappingFields(IGigyaModuleSettings settings)
        {
            if (settings.MappedMappingFields != null)
            {
                return settings.MappedMappingFields;
            }
            return !string.IsNullOrEmpty(settings.MappingFields) ? JsonConvert.DeserializeObject<List<MappingField>>(settings.MappingFields) : new List<MappingField>();
        }

        protected void ThrowTestingExceptionIfRequired(IGigyaModuleSettings settings, dynamic userInfo)
        {
            if (settings.DebugMode && DynamicUtils.GetValue<string>(userInfo, "profile.email") == Constants.Testing.EmailWhichThrowsException)
            {
                throw new ArgumentException("profile.email matches testing email which causes exception");
            }
        }

        protected abstract object ConvertCurrentSiteId(object currentSiteId);

        protected ExpandoObject GetAccountInfo(object currentSiteId, IGigyaModuleSettings settings, GSResponse userInfoResponse, List<MappingField> mappingFields)
        {
            var userInfo = JsonConvert.DeserializeObject<ExpandoObject>(userInfoResponse.GetResponseText());
            ThrowTestingExceptionIfRequired(settings, userInfo);

            var siteId = ConvertCurrentSiteId(currentSiteId);

            // fire getAccountInfo completed event
            var getAccountInfoCompletedArgs = new GetAccountInfoCompletedEventArgs
            {
                GigyaModel = userInfo,
                Settings = settings,
                Logger = _logger,
                MappingFields = mappingFields,
                CurrentSiteId = siteId
            };
            GigyaEventHub.Instance.RaiseGetAccountInfoCompleted(this, getAccountInfoCompletedArgs);

            // fire merge getAccountInfo completed event
            var accountInfoMergeCompletedArgs = new AccountInfoMergeCompletedEventArgs
            {
                GigyaModel = getAccountInfoCompletedArgs.GigyaModel,
                Settings = settings,
                Logger = _logger,
                CurrentSiteId = siteId
            };

            GigyaEventHub.Instance.RaiseAccountInfoMergeCompleted(this, accountInfoMergeCompletedArgs);
            return accountInfoMergeCompletedArgs.GigyaModel;
        }

        protected virtual string GetCmsUsername(List<MappingField> mappingFields, dynamic userInfo)
        {
            if (!mappingFields.Any())
            {
                return userInfo.UID;
            }

            return GetGigyaFieldFromCmsAlias(userInfo, CmsUserIdField, userInfo.UID, mappingFields);
        }

        protected abstract bool LoginByUsername(string username, IGigyaModuleSettings settings);

        public bool Login(string gigyaUid, IGigyaModuleSettings settings)
        {
            var username = gigyaUid;

            var uidMapping = settings.MappedMappingFields.FirstOrDefault(i => i.GigyaFieldName == Constants.GigyaFields.UserId && !string.IsNullOrEmpty(i.CmsFieldName));
            if (uidMapping == null || uidMapping.CmsFieldName != CmsUserIdField)
            {
                return false;
            }

            return LoginByUsername(username, settings);
        }

        protected abstract bool Exists(string username);

        protected abstract bool CreateUserInternal(string username, dynamic gigyaModel, IGigyaModuleSettings settings, List<MappingField> mappingFields);

        protected abstract bool AuthenticateUser(string username, IGigyaModuleSettings settings, bool updateProfile, dynamic gigyaModel, List<MappingField> mappingFields);

        /// <summary>
        /// Login or register a user.
        /// </summary>
        /// <param name="model">Details from the client e.g. signature and userId.</param>
        /// <param name="settings">Gigya module settings.</param>
        /// <param name="response">Response model that will be returned to the client.</param>
        public virtual void LoginOrRegister(LoginModel model, IGigyaModuleSettings settings, ref LoginResponseModel response)
        {
            response.Status = ResponseStatus.Error;

            if (!settings.EnableRaas)
            {
                if (settings.DebugMode)
                {
                    _logger.Debug("RaaS not enabled so login aborted.");
                }
                return;
            }

            if (!_gigyaApiHelper.ValidateApplicationKeySignature(model.UserId, settings, model.SignatureTimestamp, model.Signature))
            {
                if (settings.DebugMode)
                {
                    _logger.Debug("Invalid user signature for login request.");
                }
                return;
            }

            // get user info
            var userInfoResponse = _gigyaApiHelper.GetAccountInfo(model.UserId, settings);
            if (userInfoResponse == null || userInfoResponse.GetErrorCode() != 0)
            {
                _logger.Error("Failed to getAccountInfo");
                return;
            }

            List<MappingField> mappingFields = GetMappingFields(settings);
            var gigyaModel = GetAccountInfo(model.Id, settings, userInfoResponse, mappingFields);
            ThrowTestingExceptionIfRequired(settings, gigyaModel);

            // find what field has been configured for the CMS username
            var username = GetCmsUsername(mappingFields, gigyaModel);
            
            var userExists = Exists(username);
            if (!userExists)
            {
                var createdUser = CreateUserInternal(username, gigyaModel, settings, mappingFields);
                if (!createdUser)
                {
                    return;
                }
            }

            // user logged into Gigya so now needs to be logged into CMS
            var authenticated = AuthenticateUser(username, settings, userExists, gigyaModel, mappingFields);
            response.Status = authenticated ? ResponseStatus.Success : ResponseStatus.Error;
            if (authenticated)
            {
                response.RedirectUrl = settings.RedirectUrl;

                _gigyaAccountHelper.UpdateSessionExpirationCookieIfRequired(HttpContext.Current, true);
            }
        }
    }
}
