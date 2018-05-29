using Gigya.Module.Core.Connector.Common;
using Gigya.Module.Core.Connector.Helpers;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.Core.Connector.Models;
using Gigya.Module.Core.Data;
using Gigya.Module.Core.Mvc.Models;
using Sitecore.Gigya.Module.Pipelines;
using Sitecore.Gigya.Module.Repositories;
using Sitecore.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SC = Sitecore;
using Core = Gigya.Module.Core;
using Sitecore.Gigya.Module.Models;
using System.Configuration;
using System.Reflection;

namespace Sitecore.Gigya.Module.Helpers
{
    public class GigyaMembershipHelper : GigyaMembershipHelperBase<SitecoreGigyaModuleSettings>, IGigyaMembershipHelper<SitecoreGigyaModuleSettings>
    {
        private readonly IAccountRepository _accountRepository;

        public GigyaMembershipHelper(GigyaApiHelper<SitecoreGigyaModuleSettings> apiHelper, Logger logger, GigyaAccountHelper gigyaAccountHelper, IAccountRepository accountRepository)
            : base(apiHelper, gigyaAccountHelper, logger)
        {
            _accountRepository = accountRepository;
        }

        protected override string CmsUserIdField => Constants.CmsFields.UserId;

        public void Logout()
        {
            _accountRepository.Logout();
        }

        public void UpdateProfile(LoginModel model, SitecoreGigyaModuleSettings settings, ref LoginResponseModel response)
        {
            var userInfoResponse = ValidateRequest(model, settings);
            if (userInfoResponse == null)
            {
                return;
            }

            var currentIdentity = _accountRepository.CurrentIdentity;
            var currentUsername = currentIdentity.Name;

            List<MappingField> mappingFields = GetMappingFields(settings);

            var gigyaModel = GetAccountInfo(model.Id, settings, userInfoResponse, mappingFields);
            ThrowTestingExceptionIfRequired(settings, gigyaModel);

            var success = MapProfileFieldsAndUpdate(settings, gigyaModel, mappingFields);
            if (success)
            {
                response.RedirectUrl = settings.RedirectUrl;
                response.Status = ResponseStatus.Success;
            }
        }

        protected virtual PropertyInfo GetDefaultUserProperty(IEnumerable<PropertyInfo> properties, string name)
        {
            return properties.FirstOrDefault(i => i.Name == name);
        }

        /// <summary>
        /// Updates the users Sitecore profile.
        /// </summary>
        /// <param name="username">Id of the user to update.</param>
        /// <param name="settings">Gigya module settings for this site.</param>
        /// <param name="gigyaModel">Deserialized Gigya JSON object.</param>
        protected virtual bool MapProfileFieldsAndUpdate(SitecoreGigyaModuleSettings settings, dynamic gigyaModel, List<MappingField> mappingFields)
        {
            if (!settings.EnableMembershipSync)
            {
                return false;
            }

            if (mappingFields == null || !mappingFields.Any())
            {
                return false;
            }

            var user = _accountRepository.GetActiveUser();
            var profileTypeProperties = user.Profile.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(i => i.CanWrite).ToList();

            foreach (var field in mappingFields)
            {
                if (string.IsNullOrEmpty(field.CmsFieldName))
                {
                    _logger.Debug("Sitecore mapping field is empty.");
                    continue;
                }

                if (string.IsNullOrEmpty(field.GigyaFieldName))
                {
                    _logger.Debug("Gigya mapping field is empty.");
                    continue;
                }

                object value = GetGigyaValue(gigyaModel, field.GigyaFieldName, field.CmsFieldName);

                try
                {
                    // check if there is a property as part of the hard coded profile
                    var userProperty = GetDefaultUserProperty(profileTypeProperties, field.CmsFieldName);
                    if (userProperty != null)
                    {
                        userProperty.SetValue(user.Profile, value);
                    }
                    else
                    {
                        var gigyaValue = ConvertGigyaValueForCustomProperty(value);
                        user.Profile.SetCustomProperty(field.CmsFieldName, gigyaValue);
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(string.Format("Couldn't set profile value for [{0}] and gigya field [{1}].", field.CmsFieldName, field.GigyaFieldName), e);
                }
            }

            try
            {
                user.Profile.Save();
            }
            catch (Exception e)
            {
                _logger.Error("Failed to save profile for user: " + user.Name, e);
                return false;
            }
            return true;
        }

        protected virtual string ConvertGigyaValueForCustomProperty(object gigyaValue)
        {
            if (gigyaValue == null)
            {
                return string.Empty;
            }

            var date = gigyaValue as DateTime?;
            if (date.HasValue)
            {
                return DateUtil.ToIsoDate(date.Value);
            }

            var boolValue = gigyaValue as bool?;
            if (boolValue.HasValue)
            {
                return boolValue.Value ? "1" : "0";
            }

            return gigyaValue.ToString();
        }

        /// <summary>
        /// Gets a Gigya value from the model.
        /// </summary>
        /// <param name="gigyaModel">Deserialized Gigya JSON object.</param>
        /// <param name="gigyaFieldName">Gigya field name e.g. profile.age</param>
        /// <returns></returns>
        protected virtual object GetGigyaValue(dynamic gigyaModel, string gigyaFieldName, string sitefinityFieldName)
        {
            var args = new GigyaGetFieldEventArgs
            {
                GigyaModel = gigyaModel,
                GigyaFieldName = gigyaFieldName,
                CmsFieldName = sitefinityFieldName,
                Origin = "GetGigyaValue",
                GigyaValue = DynamicUtils.GetValue<object>(gigyaModel, gigyaFieldName)
            };

            CorePipeline.Run("gigya.module.getGigyaValue", args, false);
            return args.GigyaValue;
        }

        /// <summary>
        /// In the Sitefinity module the current site id is passed as an array so we need to convert to an int.
        /// </summary>
        protected override object ConvertCurrentSiteId(object currentSiteId)
        {
            return currentSiteId;
        }

        /// <summary>
        /// Gets the Gigya UID for the current logged in user. If the user isn't logged in, null is returned.
        /// </summary>
        /// <param name="settings">Current site settings.</param>
        /// <returns>The UID value.</returns>
        public string GetUidForCurrentUser(GigyaModuleSettings settings)
        {
            // check user is logged in
            var currentIdentity = SC.Context.User;
            if (!currentIdentity.IsAuthenticated)
            {
                return null;
            }

            return currentIdentity.Name;
        }

        protected override bool LoginByUsername(string username, SitecoreGigyaModuleSettings settings)
        {
            var persistent = PersistentAuthRequired(settings);
            return _accountRepository.Login(username, persistent) != null;
        }

        protected override bool Exists(string username)
        {
            return _accountRepository.Exists(username);
        }

        protected override bool CreateUserInternal(string username, dynamic gigyaModel, SitecoreGigyaModuleSettings settings, List<MappingField> mappingFields)
        {
            var persistent = PersistentAuthRequired(settings);
            var email = DynamicUtils.GetValue<string>(gigyaModel, Core.Constants.GigyaFields.Email);
            var password = SecurityUtils.CreateCryptographicallySecureGuid().ToString();

            _accountRepository.Register(username, email, password, persistent, settings.ProfileId);
            return true;
        }

        protected override bool AuthenticateUser(string username, SitecoreGigyaModuleSettings settings, bool updateProfile, dynamic gigyaModel, List<MappingField> mappingFields)
        {
            var isLoggedIn = LoginByUsername(username, settings);
            if (!isLoggedIn)
            {
                return false;
            }

            if (updateProfile)
            {
                return MapProfileFieldsAndUpdate(settings, gigyaModel, mappingFields);
            }

            return true;
        }
    }
}
