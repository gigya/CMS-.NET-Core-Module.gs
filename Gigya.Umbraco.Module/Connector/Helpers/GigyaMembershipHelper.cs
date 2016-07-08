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

namespace Gigya.Umbraco.Module.Connector.Helpers
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
        protected void UpdateProfile(string userId, IGigyaModuleSettings settings, dynamic gigyaModel)
        {
            var memberService = U.Core.ApplicationContext.Current.Services.MemberService;
            var user = memberService.GetByUsername(userId);
            user.Email = GetGigyaValue(gigyaModel, Constants.GigyaFields.Email, Constants.CmsFields.Email);

            // map any custom fields
            MapProfileFields(user, gigyaModel, settings);

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
        protected virtual void MapProfileFields(IMember user, dynamic gigyaModel, IGigyaModuleSettings settings)
        {
            // map custom fields
            if (!string.IsNullOrEmpty(settings.MappingFields))
            {
                var mappingFields = JsonConvert.DeserializeObject<List<MappingField>>(settings.MappingFields);
                foreach (var field in mappingFields)
                {
                    // check if field is a custom one
                    switch (field.CmsFieldName)
                    {
                        case Constants.CmsFields.Email:
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
            
            IMember user = memberService.CreateMemberWithIdentity(userId, email, email, Constants.MemberTypeAlias);
            if (user == null)
            {
                return null;
            }

            MapProfileFields(user, gigyaModel, settings);
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
            
            var memberService = U.Core.ApplicationContext.Current.Services.MemberService;
            var userExists = memberService.Exists(model.UserId);
            if (!userExists)
            {
                // user doesn't exist so create a new one
                var user = CreateUser(model.UserId, gigyaModel, settings);
                if (user == null)
                {
                    return;
                }
            }

            // user logged into Gigya so now needs to be logged into Umbraco
            var authenticated = AuthenticateUser(model, settings, userExists, gigyaModel);
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

            dynamic userInfo = JsonConvert.DeserializeObject<ExpandoObject>(userInfoResponse.GetResponseText());
            ThrowTestingExceptionIfRequired(settings, userInfo);
            
            UpdateProfile(model.UserId, settings, userInfo);
            response.RedirectUrl = settings.RedirectUrl;
            response.Status = ResponseStatus.Success;
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
        protected virtual bool AuthenticateUser(LoginModel model, IGigyaModuleSettings settings, bool updateProfile, dynamic gigyaModel)
        {
            FormsAuthentication.SetAuthCookie(model.UserId, false);

            if (settings.DebugMode)
            {
                _logger.Debug(string.Concat("User [", model.UserId, "] successfully logged into Umbraco."));
            }

            if (updateProfile)
            {
                UpdateProfile(model.UserId, settings, gigyaModel);
            }
            return true;
        }
    }
}
