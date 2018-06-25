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
using Sitecore.Security.Accounts;
using Sitecore.DependencyInjection;
using Sitecore.Gigya.Extensions.Abstractions.Services;
using Sitecore.Gigya.Extensions.Services;

namespace Sitecore.Gigya.Module.Helpers
{
    public class GigyaMembershipHelper : GigyaMembershipHelperBase<SitecoreGigyaModuleSettings>, IGigyaMembershipHelper<SitecoreGigyaModuleSettings>
    {
        protected readonly IAccountRepository _accountRepository;
        protected readonly ITrackerService _trackerService;
        protected readonly IContactProfileService _contactProfileService;

        public GigyaMembershipHelper(GigyaApiHelper<SitecoreGigyaModuleSettings> apiHelper, Logger logger, GigyaAccountHelper gigyaAccountHelper, 
            IAccountRepository accountRepository, ITrackerService trackerService, IContactProfileService contactProfileService)
            : base(apiHelper, gigyaAccountHelper, logger)
        {
            _accountRepository = accountRepository;
            _trackerService = trackerService;
            _contactProfileService = contactProfileService;
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
            var user = _accountRepository.GetActiveUser();
            var success = MapProfileFieldsAndUpdate(settings, gigyaModel, mappingFields, user);
            UpdateXdb(settings, gigyaModel);
            return success;
        }

        /// <summary>
        /// Updates the users Sitecore profile.
        /// </summary>
        /// <param name="username">Id of the user to update.</param>
        /// <param name="settings">Gigya module settings for this site.</param>
        /// <param name="gigyaModel">Deserialized Gigya JSON object.</param>
        /// <param name="user">The user to update.</param>
        protected virtual bool MapProfileFieldsAndUpdate(SitecoreGigyaModuleSettings settings, dynamic gigyaModel, List<MappingField> mappingFields, User user)
        {
            if (!settings.EnableMembershipSync)
            {
                return true;
            }

            if (mappingFields == null || !mappingFields.Any())
            {
                return true;
            }

            // profileId is different
            if (user.Profile.ProfileItemId != settings.ProfileId)
            {
                user.Profile.ProfileItemId = settings.ProfileId;

                try
                {
                    user.Profile.Save();
                }
                catch (Exception e)
                {
                    _logger.Error("Failed to update profile for user: " + user.Name, e);
                    return false;
                }
            }

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

                try
                {
                    object value = GetGigyaValue(gigyaModel, field.GigyaFieldName, field.CmsFieldName);

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
        protected virtual object GetGigyaValue(dynamic gigyaModel, string gigyaFieldName, string cmsFieldName)
        {
            var args = new GigyaGetFieldEventArgs
            {
                GigyaModel = gigyaModel,
                GigyaFieldName = gigyaFieldName,
                CmsFieldName = cmsFieldName,
                Origin = "GetGigyaValue",
                GigyaValue = DynamicUtils.GetValue<object>(gigyaModel, gigyaFieldName)
            };

            if (args.GigyaValue == null)
            {
                // apply any custom computed fields
                switch (args.GigyaFieldName)
                {
                    case Core.Constants.GigyaFields.FullName:
                        args.GigyaValue = string.Join(" ", DynamicUtils.GetValue<string>(gigyaModel, Core.Constants.GigyaFields.FirstName), DynamicUtils.GetValue<string>(gigyaModel, Core.Constants.GigyaFields.LastName)).Trim();
                        break;
                }
            }

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
            var user = _accountRepository.Register(username, email, password, persistent, settings.ProfileId);

            MapProfileFieldsAndUpdate(settings, gigyaModel, mappingFields, user);
            return true;
        }

        protected override bool AuthenticateUser(string username, SitecoreGigyaModuleSettings settings, bool updateProfile, dynamic gigyaModel, List<MappingField> mappingFields)
        {
            var success = LoginByUsername(username, settings);
            if (!success)
            {
                return false;
            }

            if (updateProfile)
            {
                success = MapProfileFieldsAndUpdate(settings, gigyaModel, mappingFields);
            }
            else
            {
                UpdateXdb(settings, gigyaModel);
            }

            return success;
        }

        private void UpdateXdb(SitecoreGigyaModuleSettings settings, dynamic gigyaModel)
        {
            if (settings.EnableXdb)
            {
                // identify contact                
                _trackerService.IdentifyContact(_accountRepository.CurrentIdentity.Name);

                // update the contacts facets
                _contactProfileService.UpdateFacetsAsync(gigyaModel, settings.MappedXdbMappingFields).Wait();
            }
        }
    }
}
