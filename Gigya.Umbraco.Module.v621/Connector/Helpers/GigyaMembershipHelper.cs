using Gigya.Module.Core.Connector.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gigya.Module.Core.Data;
using Gigya.Module.Core.Mvc.Models;
using Gigya.Module.Core.Connector.Common;
using Gigya.Module.Core.Connector.Logging;
using System.Dynamic;
using Newtonsoft.Json;
using Gigya.Module.Core.Connector.Models;
using Gigya.Module.Core.Connector.Events;

using U = Umbraco;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using System.Web.Security;

namespace Gigya.Umbraco.Module.v621.Connector.Helpers
{
    public class GigyaMembershipHelper : IGigyaMembershipHelper
    {
        public static event EventHandler<MapGigyaFieldEventArgs> GettingGigyaValue;
        private readonly GigyaApiHelper _gigyaApiHelper;
        private readonly Logger _logger;

        public GigyaMembershipHelper(GigyaApiHelper apiHelper, Logger logger)
        {
            _gigyaApiHelper = apiHelper;
            _logger = logger;
        }

        /// <summary>
        /// Updates the users Umbraco profile.
        /// </summary>
        /// <param name="userId">Id of the user to update.</param>
        /// <param name="settings">Gigya module settings for this site.</param>
        /// <param name="gigyaModel">Deserialized Gigya JSON object.</param>
        protected void MapProfileFieldsAndUpdate(string userId, IGigyaModuleSettings settings, dynamic gigyaModel, List<MappingField> mappingFields)
        {
            var memberService = U.Core.ApplicationContext.Current.Services.MemberService;
            var user = memberService.GetByUsername(userId);
            user.Email = GetGigyaValue(gigyaModel, Constants.GigyaFields.Email, Constants.CmsFields.Email);

            // map any custom fields
            MapProfileFields(user, gigyaModel, settings, mappingFields);

            try
            {
                memberService.Save(user);
            }
            catch (Exception e)
            {
                _logger.Error("Failed to update profile for userId: " + userId, e);
            }
        }

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

        /// <summary>
        /// Gets a Gigya value from the model.
        /// </summary>
        /// <param name="gigyaModel">Deserialized Gigya JSON object.</param>
        /// <param name="gigyaFieldName">Gigya field name e.g. profile.age</param>
        /// <returns></returns>
        protected object GetGigyaValue(dynamic gigyaModel, string gigyaFieldName, string cmsFieldName)
        {
            var eventArgs = new MapGigyaFieldEventArgs
            {
                GigyaModel = gigyaModel,
                GigyaFieldName = gigyaFieldName,
                CmsFieldName = cmsFieldName,
                Origin = "GetGigyaValue",
                GigyaValue = DynamicUtils.GetValue<object>(gigyaModel, gigyaFieldName)
            };

            GettingGigyaValue?.Invoke(this, eventArgs);
            return eventArgs.GigyaValue;
        }

        /// <summary>
        /// Maps Gigya fields to a Umbraco profile.
        /// </summary>
        /// <param name="profile">The profile to update.</param>
        /// <param name="gigyaModel">Deserialized Gigya JSON object.</param>
        /// <param name="settings">The Gigya module settings.</param>
        protected virtual void MapProfileFields(IMember user, dynamic gigyaModel, IGigyaModuleSettings settings, List<MappingField> mappingFields)
        {
            if (mappingFields == null && string.IsNullOrEmpty(settings.MappingFields))
            {
                return;
            }

            // map custom fields
            foreach (var field in mappingFields)
            {
                // check if field is a custom one
                switch (field.CmsFieldName)
                {
                    case Constants.CmsFields.Email:
                    case Constants.CmsFields.Name:
                    case Constants.CmsFields.Username:
                        continue;
                }

                if (!string.IsNullOrEmpty(field.CmsFieldName) && user.HasProperty(field.CmsFieldName))
                {
                    object gigyaValue = GetGigyaValue(gigyaModel, field.GigyaFieldName, field.CmsFieldName);
                    if (gigyaValue != null)
                    {
                        try
                        {
                            user.SetValue(field.CmsFieldName, gigyaValue);
                        }
                        catch (Exception e)
                        {
                            _logger.Error(string.Format("Couldn't set Umbraco profile value for [{0}] and gigya field [{1}].", field.CmsFieldName, field.GigyaFieldName), e);
                            continue;
                        }

                        if (user.GetValue(field.CmsFieldName).ToString() != gigyaValue.ToString())
                        {
                            _logger.Error(string.Format("Umbraco field [{0}] type doesn't match Gigya field [{1}] type. You may need to add a conversion using GigyaMembershipHelper.GettingGigyaValue", field.CmsFieldName, field.GigyaFieldName));
                        }
                    }
                    else if (settings.DebugMode)
                    {   
                        _logger.DebugFormat("Gigya field \"{0}\" is null so profile field not updated.", field.GigyaFieldName);
                    }
                }
                else if (settings.DebugMode)
                {
                    _logger.DebugFormat("Umbraco field \"{0}\" not found.", field.CmsFieldName);
                }
            }
        }

        /// <summary>
        /// Creates a new Umbraco user from a Gigya user.
        /// </summary>
        /// <param name="userId">The Id of the new user.</param>
        /// <param name="gigyaModel">Deserialized Gigya JSON object.</param>
        /// <param name="settings">Gigya module settings for the site.</param>
        /// <returns></returns>
        protected virtual IMember CreateUser(string userId, dynamic gigyaModel, IGigyaModuleSettings settings)
        {
            var memberService = U.Core.ApplicationContext.Current.Services.MemberService;
            var email = GetGigyaValueWithDefault(gigyaModel, Constants.GigyaFields.Email, null);
            var name = email;

            // check if there is a name field for the member name otherwise fallback to email
            List<MappingField> mappingFields = null;
            if (!string.IsNullOrEmpty(settings.MappingFields))
            {
                mappingFields = JsonConvert.DeserializeObject<List<MappingField>>(settings.MappingFields);
                name = GetGigyaFieldFromCmsAlias(gigyaModel, Constants.CmsFields.Name, email, mappingFields);
            }

            IMember user = memberService.CreateMemberWithIdentity(userId, email, name, Constants.MemberTypeAlias);
            if (user == null)
            {
                return null;
            }

            MapProfileFields(user, gigyaModel, settings, mappingFields);
            try
            {
                memberService.Save(user);
            }
            catch (Exception e)
            {
                _logger.Error("Failed to update profile for userId: " + userId, e);
            }
            
            return user;
        }

        private string GetGigyaFieldFromCmsAlias(dynamic gigyaModel, string cmsFieldName, string fallback, List<MappingField> mappingFields)
        {
            var field = mappingFields.FirstOrDefault(i => i.CmsFieldName == cmsFieldName);
            if (field != null && !string.IsNullOrEmpty(field.GigyaFieldName))
            {
                return GetGigyaValueWithDefault(gigyaModel, field.GigyaFieldName, fallback);
            }

            return fallback;
        }

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

            var gigyaModel = JsonConvert.DeserializeObject<ExpandoObject>(userInfoResponse.GetResponseText());
            ThrowTestingExceptionIfRequired(settings, gigyaModel);

            // find what field has been configured for the Umbraco username
            List<MappingField> mappingFields = GetMappingFields(settings);
            var username = GetUmbracoUsername(mappingFields, gigyaModel);

            var memberService = U.Core.ApplicationContext.Current.Services.MemberService;
            var userExists = memberService.Exists(username);
            if (!userExists)
            {
                // user doesn't exist so create a new one
                var user = CreateUser(username, gigyaModel, settings);
                if (user == null)
                {
                    return;
                }
            }

            // user logged into Gigya so now needs to be logged into Umbraco
            var authenticated = AuthenticateUser(username, settings, userExists, gigyaModel, mappingFields);
            response.Status = authenticated ? ResponseStatus.Success : ResponseStatus.Error;
            if (authenticated)
            {
                response.RedirectUrl = settings.RedirectUrl;
            }
        }

        /// <summary>
        /// Updates a user's profile in Umbraco.
        /// </summary>
        public virtual void UpdateProfile(LoginModel model, IGigyaModuleSettings settings, ref LoginResponseModel response)
        {
            if (!settings.EnableRaas)
            {
                if (settings.DebugMode)
                {
                    _logger.Debug("RaaS not enabled so login aborted.");
                }
                return;
            }

            var userInfoResponse = _gigyaApiHelper.GetAccountInfo(model.UserId, settings);
            if (userInfoResponse == null || userInfoResponse.GetErrorCode() != 0)
            {
                if (settings.DebugMode)
                {
                    _logger.Error("Failed to getAccountInfo");
                }
                return;
            }

            List<MappingField> mappingFields = GetMappingFields(settings);

            dynamic userInfo = JsonConvert.DeserializeObject<ExpandoObject>(userInfoResponse.GetResponseText());
            ThrowTestingExceptionIfRequired(settings, userInfo);

            string username = GetUmbracoUsername(mappingFields, userInfo);
            MapProfileFieldsAndUpdate(username, settings, userInfo, mappingFields);
            response.RedirectUrl = settings.RedirectUrl;
            response.Status = ResponseStatus.Success;
        }

        private static List<MappingField> GetMappingFields(IGigyaModuleSettings settings)
        {
            return !string.IsNullOrEmpty(settings.MappingFields) ? JsonConvert.DeserializeObject<List<MappingField>>(settings.MappingFields) : new List<MappingField>();
        }

        private string GetUmbracoUsername(List<MappingField> mappingFields, dynamic userInfo)
        {
            if (!mappingFields.Any())
            {
                return userInfo.UID;
            }

            return GetGigyaFieldFromCmsAlias(userInfo, Constants.CmsFields.Username, userInfo.UID, mappingFields);
        }

        private void ThrowTestingExceptionIfRequired(IGigyaModuleSettings settings, dynamic userInfo)
        {
            if (settings.DebugMode && DynamicUtils.GetValue<string>(userInfo, "profile.email") == Constants.Testing.EmailWhichThrowsException)
            {
                throw new ArgumentException("profile.email matches testing email which causes exception");
            }
        }

        /// <summary>
        /// Authenticates a user in Umbraco.
        /// </summary>
        protected virtual bool AuthenticateUser(string username, IGigyaModuleSettings settings, bool updateProfile, dynamic gigyaModel, List<MappingField> mappingFields)
        {
            FormsAuthentication.SetAuthCookie(username, false);

            if (settings.DebugMode)
            {
                _logger.Debug(string.Concat("User [", username, "] successfully logged into Umbraco."));
            }

            if (updateProfile)
            {
                MapProfileFieldsAndUpdate(username, settings, gigyaModel, mappingFields);
            }
            return true;
        }
    }
}
