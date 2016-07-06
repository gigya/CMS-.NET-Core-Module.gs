using Gigya.Module.Data;
using Gigya.Module.Mvc.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Model;
using Telerik.Sitefinity.Model;
using Gigya.Module.Connector.Logging;
using Telerik.Sitefinity.Data;
using Gigya.Socialize.SDK;
using Gigya.Module.Connector.Models;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using Gigya.Module.Connector.Common;
using Telerik.Sitefinity.Services.Events;
using Telerik.Sitefinity.Services;
using Gigya.Module.Connector.Events;

namespace Gigya.Module.Connector.Helpers
{
    public class GigyaMembershipHelper
    {
        /// <summary>
        /// Updates the users Sitefinity profile.
        /// </summary>
        /// <param name="userId">Id of the user to update.</param>
        /// <param name="settings">Gigya module settings for this site.</param>
        /// <param name="gigyaModel">Deserialized Gigya JSON object.</param>
        protected virtual void UpdateProfile(string userId, GigyaModuleSettings settings, dynamic gigyaModel)
        {
            UserProfileManager profileManager = UserProfileManager.GetManager();
            UserManager userManager = UserManager.GetManager();

            using (new ElevatedModeRegion(userManager))
            {
                var user = userManager.GetUser(userId);
                user.Email = GetGigyaValue(gigyaModel, Constants.GigyaFields.Email, Constants.SitefinityFields.Email);

                SitefinityProfile profile = profileManager.GetUserProfile<SitefinityProfile>(user);
                
                if (profile == null)
                {
                    profile = profileManager.CreateProfile(user, Guid.NewGuid(), typeof(SitefinityProfile)) as SitefinityProfile;

                    // only set this on creation as it's possible to get 2 users with the same email address
                    profile.Nickname = user.Email;
                }               

                // map any custom fields
                MapProfileFields(profile, gigyaModel, settings);
                
                try
                {
                    userManager.SaveChanges();
                    profileManager.SaveChanges();
                }
                catch(Exception e)
                {
                    Logger.Error("Failed to update profile for userId: " + userId, e);
                }
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
        protected virtual object GetGigyaValue(dynamic gigyaModel, string gigyaFieldName, string sitefinityFieldName)
        {
            var eventArgs = new MapGigyaFieldEvent
            {
                GigyaModel = gigyaModel,
                GigyaFieldName = gigyaFieldName,
                SitefinityFieldName = sitefinityFieldName,
                Origin = "GetGigyaValue",
                GigyaValue = DynamicUtils.GetValue<object>(gigyaModel, gigyaFieldName)
            };
            EventHub.Raise(eventArgs);

            return eventArgs.GigyaValue;
        }

        /// <summary>
        /// Maps Gigya fields to a Sitefinity profile.
        /// </summary>
        /// <param name="profile">The profile to update.</param>
        /// <param name="gigyaModel">Deserialized Gigya JSON object.</param>
        /// <param name="settings">The Gigya module settings.</param>
        protected virtual void MapProfileFields(SitefinityProfile profile, dynamic gigyaModel, GigyaModuleSettings settings)
        {
            profile.FirstName = GetGigyaValue(gigyaModel, Constants.GigyaFields.FirstName, Constants.SitefinityFields.FirstName);
            profile.LastName = GetGigyaValue(gigyaModel, Constants.GigyaFields.LastName, Constants.SitefinityFields.LastName);

            // map custom fields
            if (!string.IsNullOrEmpty(settings.MappingFields))
            {
                var mappingFields = JsonConvert.DeserializeObject<List<MappingField>>(settings.MappingFields);
                foreach (var field in mappingFields)
                {
                    // check if field is a custom one
                    switch (field.SitefinityFieldName)
                    {
                        case Constants.SitefinityFields.FirstName:
                        case Constants.SitefinityFields.LastName:
                        case Constants.SitefinityFields.Email:
                            continue;
                    }

                    if (!string.IsNullOrEmpty(field.SitefinityFieldName) && profile.DoesFieldExist(field.SitefinityFieldName))
                    {
                        object gigyaValue = GetGigyaValue(gigyaModel, field.GigyaFieldName, field.SitefinityFieldName);
                        if (gigyaValue != null)
                        {
                            try
                            {
                                profile.SetValue(field.SitefinityFieldName, gigyaValue);
                            }
                            catch (Exception e)
                            {
                                Logger.Error(string.Format("Couldn't set Sitefinity profile value for [{0}] and gigya field [{1}].", field.SitefinityFieldName, field.GigyaFieldName), e);
                            }

                            if (profile.GetValue(field.SitefinityFieldName) != gigyaValue)
                            {
                                Logger.Error(string.Format("Sitefinity field [{0}] type doesn't match Gigya field [{1}] type. You may need to add a conversion using EventHub.Subscribe<IMapGigyaFieldEvent>", field.SitefinityFieldName, field.GigyaFieldName));
                            }
                        }
                        else if (settings.DebugMode)
                        {
                            Logger.DebugFormat("Gigya field \"{0}\" is null so profile field not updated.", field.GigyaFieldName);
                        }
                    }
                    else if (settings.DebugMode)
                    {
                        Logger.DebugFormat("Sitefinity field \"{0}\" not found.", field.SitefinityFieldName);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new Sitefinity user from a Gigya user.
        /// </summary>
        /// <param name="userId">The Id of the new user.</param>
        /// <param name="gigyaModel">Deserialized Gigya JSON object.</param>
        /// <param name="settings">Gigya module settings for the site.</param>
        /// <returns></returns>
        protected virtual MembershipCreateStatus CreateUser(string userId, dynamic gigyaModel, GigyaModuleSettings settings)
        {
            UserManager userManager = UserManager.GetManager();
            UserProfileManager profileManager = UserProfileManager.GetManager();

            MembershipCreateStatus status;
            var email = GetGigyaValueWithDefault(gigyaModel, Constants.GigyaFields.Email, null);
            var firstName = GetGigyaValueWithDefault(gigyaModel, Constants.GigyaFields.FirstName, "First Name");
            var lastName = GetGigyaValueWithDefault(gigyaModel, Constants.GigyaFields.LastName, "Last Name");

            User user = userManager.CreateUser(userId, Guid.NewGuid().ToString(), email, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), true, null, out status);

            switch (status)
            {
                case MembershipCreateStatus.Success:
                    CreateProfile(gigyaModel, settings, userManager, profileManager, user);
                    break;
                case MembershipCreateStatus.DuplicateEmail:
                    // insert fails if there is a duplicate email even though duplicate emails are allowed...strange huh
                    var dummyEmail = string.Concat(Guid.NewGuid(), "@gigya.com");

                    // try insert again with a dummy email and update it afterwards
                    user = userManager.CreateUser(userId, Guid.NewGuid().ToString(), dummyEmail, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), true, null, out status);
                    if (status == MembershipCreateStatus.Success)
                    {
                        user.Email = email;
                        CreateProfile(gigyaModel, settings, userManager, profileManager, user);
                    }
                    break;
            }

            return status;
        }

        /// <summary>
        /// Creates a new Sitefinity profile.
        /// </summary>
        /// <param name="gigyaModel">Deserialized Gigya JSON object.</param>
        /// <param name="settings">Gigya module settings.</param>
        /// <param name="userManager">Sitefinity user manager.</param>
        /// <param name="profileManager">Sitefinity profile manager.</param>
        /// <param name="user">The user that will be associated with the new profile.</param>
        protected virtual void CreateProfile(dynamic gigyaModel, GigyaModuleSettings settings, UserManager userManager, UserProfileManager profileManager, User user)
        {
            SitefinityProfile profile = profileManager.CreateProfile(user, Guid.NewGuid(), typeof(SitefinityProfile)) as SitefinityProfile;

            if (profile != null)
            {
                profile.Nickname = user.Email;

                userManager.SaveChanges();
                profileManager.RecompileItemUrls(profile);
                profileManager.SaveChanges();

                MapProfileFields(profile, gigyaModel, settings);
            }

            try
            {
                profileManager.RecompileItemUrls(profile);
                profileManager.SaveChanges();
            }
            catch (Exception e)
            {
                Logger.Error("Failed to create profile for userId: " + user.Id, e);
            }
        }

        /// <summary>
        /// Login or register a user.
        /// </summary>
        /// <param name="model">Details from the client e.g. signature and userId.</param>
        /// <param name="settings">Gigya module settings.</param>
        /// <param name="response">Response model that will be returned to the client.</param>
        public virtual void LoginOrRegister(LoginModel model, GigyaModuleSettings settings, ref LoginResponseModel response)
        {
            response.Status = ResponseStatus.Error;
            
            if (!settings.EnableRaas)
            {
                if (settings.DebugMode)
                {
                    Logger.Debug("RaaS not enabled so login aborted.");
                }
                return;
            }
            
            if (!GigyaApiHelper.ValidateApplicationKeySignature(model.UserId, settings, model.SignatureTimestamp, model.Signature))
            {
                if (settings.DebugMode)
                {
                    Logger.Debug("Invalid user signature for login request.");
                }
                return;
            }

            // get user info
            var userInfoResponse = GigyaApiHelper.GetAccountInfo(model.UserId, settings);
            if (userInfoResponse == null || userInfoResponse.GetErrorCode() != 0)
            {
                Logger.Error("Failed to getAccountInfo");
                return;
            }
            
            var gigyaModel = JsonConvert.DeserializeObject<ExpandoObject>(userInfoResponse.GetResponseText());
            ThrowTestingExceptionIfRequired(settings, gigyaModel);

            UserManager manager = UserManager.GetManager();
            var userExists = manager.UserExists(model.UserId);
            if (!userExists)
            {
                // user doesn't exist so create a new one
                using (new ElevatedModeRegion(manager))
                {
                    var createUserStatus = CreateUser(model.UserId, gigyaModel, settings);
                    if (createUserStatus != MembershipCreateStatus.Success)
                    {
                        return;
                    }
                }
            }

            // user logged into Gigya so now needs to be logged into Sitefinity
            var authenticated = AuthenticateUser(model, settings, userExists, gigyaModel);
            response.Status = authenticated ? ResponseStatus.Success : ResponseStatus.Error;
            if (authenticated)
            {
                response.RedirectUrl = settings.RedirectUrl;
            }
        }

        /// <summary>
        /// Updates a user's profile in Sitefinity.
        /// </summary>
        public virtual void UpdateProfile(LoginModel model, GigyaModuleSettings settings, ref LoginResponseModel response)
        {
            if (!settings.EnableRaas)
            {
                if (settings.DebugMode)
                {
                    Logger.Debug("RaaS not enabled so login aborted.");
                }
                return;
            }

            var userInfoResponse = GigyaApiHelper.GetAccountInfo(model.UserId, settings);
            if (userInfoResponse == null || userInfoResponse.GetErrorCode() != 0)
            {
                if (settings.DebugMode)
                {
                    Logger.Error("Failed to getAccountInfo");
                }
                return;
            }

            dynamic userInfo = JsonConvert.DeserializeObject<ExpandoObject>(userInfoResponse.GetResponseText());
            ThrowTestingExceptionIfRequired(settings, userInfo);

            UpdateProfile(model, userInfo, settings, ref response);
        }

        private void ThrowTestingExceptionIfRequired(GigyaModuleSettings settings, dynamic userInfo)
        {
            if (settings.DebugMode && DynamicUtils.GetValue<string>(userInfo, "profile.email") == Constants.Testing.EmailWhichThrowsException)
            {
                throw new ArgumentException("profile.email matches testing email which causes exception");
            }
        }

        /// <summary>
        /// Updates a user's profile in Sitefinity.
        /// </summary>
        public virtual void UpdateProfile(LoginModel model, dynamic gigyaModel, GigyaModuleSettings settings, ref LoginResponseModel response)
        {
            response.Status = ResponseStatus.Error;

            if (!settings.EnableRaas)
            {
                if (settings.DebugMode)
                {
                    Logger.Debug("RaaS not enabled so login aborted.");
                }
                return;
            }

            UpdateProfile(model.UserId, settings, gigyaModel);
            response.RedirectUrl = settings.RedirectUrl;
            response.Status = ResponseStatus.Success;
        }

        /// <summary>
        /// Authenticates a user in Sitefinity.
        /// </summary>
        protected virtual bool AuthenticateUser(LoginModel model, GigyaModuleSettings settings, bool updateProfile, dynamic gigyaModel)
        {
            User user;

            var loginStatus = AuthenticateWithRetry(model.UserId, 0, 2, out user);
            switch (loginStatus)
            {
                case UserLoggingReason.Success:
                    if (settings.DebugMode)
                    {
                        Logger.Debug(string.Concat("User [", model.UserId, "] successfully logged into Sitefinity."));
                    }

                    if (updateProfile)
                    {
                        UpdateProfile(model.UserId, settings, gigyaModel);
                    }
                    return true;
                default:
                    Logger.Error(string.Format("User [{0}] not logged into Sitefinity. Reason: {1}.", model.UserId, loginStatus));
                    return false;
            }
        }

        /// <summary>
        /// Authenticates user with the ability to retry if an attempt fails.
        /// </summary>
        /// <param name="userId">UserId to authenticate.</param>
        /// <param name="attempts">Number to attempts so far.</param>
        /// <param name="retries">Number of retries allowed.</param>
        /// <param name="user">The authenticated user.</param>
        /// <returns></returns>
        protected virtual UserLoggingReason AuthenticateWithRetry(string userId, int attempts, int retries, out User user)
        {
            attempts++;
            var loginStatus = SecurityManager.AuthenticateUser(null, userId, false, out user);
            switch (loginStatus)
            {
                case UserLoggingReason.Success:
                    return loginStatus;
                default:
                    if (attempts < retries)
                    {
                        return AuthenticateWithRetry(userId, attempts, retries, out user);
                    }
                    return loginStatus;
            }
        }
    }
}