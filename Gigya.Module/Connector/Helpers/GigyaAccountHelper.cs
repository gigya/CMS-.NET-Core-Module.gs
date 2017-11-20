using Gigya.Module.Connector.Logging;
using Gigya.Module.Core.Connector.Enums;
using Gigya.Module.Core.Connector.Helpers;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.Core.Data;
using Gigya.Module.Core.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Claims;
using Telerik.Sitefinity.Security.Model;
using Telerik.Sitefinity.Web;

namespace Gigya.Module.Connector.Helpers
{
    public class GigyaAccountHelper : Gigya.Module.Core.Connector.Helpers.GigyaAccountHelperBase
    {
        private const string _checkedIfLoginRequiredKey = "GigyaAccountHelper.CheckedIfLoginRequiredKey";
        private const string _executedProcessRequestKey = "GigyaAccountHelper.ExecutedProcessRequestKey";

        public GigyaAccountHelper(GigyaSettingsHelper settingsHelper, Logger logger, IGigyaMembershipHelper membershipHelper, IGigyaModuleSettings settings = null) : base(settingsHelper, logger, membershipHelper, settings)
        {
        }

        /// <summary>
        /// Creates a new GigyaAccountHelper instance and calls LoginToGigyaIfRequired. This will only be run once per request no matter how many times it's called.
        /// </summary>
        [Obsolete("Use ProcessRequestCheck instead.")]
        public static void ValidateAndLoginToGigyaIfRequired(HttpContext context, IGigyaModuleSettings settings = null)
        {
            if (context.Items.Contains(_checkedIfLoginRequiredKey))
            {
                return;
            }

            context.Items[_checkedIfLoginRequiredKey] = true;

            var settingsHelper = new GigyaSettingsHelper();
            var logger = LoggerFactory.Instance();
            var membershipHelper = new GigyaMembershipHelper(new GigyaApiHelper(settingsHelper, logger), logger);
            var accountHelper = new GigyaAccountHelper(settingsHelper, logger, membershipHelper, settings);

            accountHelper.LoginToGigyaIfRequired();
        }

        /// <summary>
        /// Executes checks which need to run for every request e.g. notifyLogin if CMS is running the session and extending the session if Gigya is managing it.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="settings"></param>
        public static void ProcessRequestChecks(HttpContext context, IGigyaModuleSettings settings = null)
        {
            var currentNode = SiteMapBase.GetCurrentNode();
            if (currentNode != null && currentNode.IsBackend)
            {
                return;
            }

            // don't need to do anything if the user is editing content in the CMS
            var identity = ClaimsManager.GetCurrentIdentity();
            if (identity.IsBackendUser)
            {
                return;
            }

            if (context.Items.Contains(_executedProcessRequestKey))
            {
                return;
            }

            context.Items[_executedProcessRequestKey] = true;

            var settingsHelper = new GigyaSettingsHelper();
            var logger = LoggerFactory.Instance();
            var membershipHelper = new GigyaMembershipHelper(new GigyaApiHelper(settingsHelper, logger), logger);
            var accountHelper = new GigyaAccountHelper(settingsHelper, logger, membershipHelper, settings);

            accountHelper.LoginToGigyaIfRequired();
            accountHelper.UpdateSessionExpirationCookieIfRequired(context);
        }

        /// <summary>
        /// Logs a user into Gigya by calling NotifyLogin.
        /// </summary>
        /// <param name="currentIdentity"></param>
        public override void LoginToGigyaIfRequired()
        {
            if (!_settings.EnableRaas)
            {
                _logger.Error("RaaS not enabled.");
                return;
            }

            if (_settings.SessionProvider != GigyaSessionProvider.CMS)
            {
                return;
            }

            var identity = ClaimsManager.GetCurrentIdentity();
            var currentIdentity = new CurrentIdentity
            {
                IsAuthenticated = identity.IsAuthenticated,
                Name = identity.Name,
                UID = identity.Name
            };

            var cookieName = "glt_" + _settings.ApiKey;
            if (!currentIdentity.IsAuthenticated || HttpContext.Current.Request.Cookies[cookieName] != null)
            {
                // user not logged in
                return;
            }

            // get UID if not the username
            var uidMapping = _settings.MappedMappingFields.FirstOrDefault(i => i.GigyaFieldName == Constants.GigyaFields.UserId && !string.IsNullOrEmpty(i.CmsFieldName));
            if (uidMapping != null && uidMapping.CmsFieldName != Constants.SitefinityFields.UserId)
            {
                // get member to find UID field
                var userManager = UserManager.GetManager();
                var currentUser = userManager.GetUser(identity.UserId);
                if (currentUser == null)
                {
                    _logger.Error(string.Format("Couldn't find member with username of {0} so couldn't sign them in.", currentIdentity.Name));
                    return;
                }

                var profileManager = UserProfileManager.GetManager();
                var profile = profileManager.GetUserProfile<SitefinityProfile>(currentUser);
                if (profile == null)
                {
                    _logger.Error(string.Format("Couldn't find profile for member with username of {0} so couldn't sign them in.", currentIdentity.Name));
                    return;
                }

                currentIdentity.UID = profile.GetValue<string>(uidMapping.CmsFieldName);
            }
            
            // user logged into Umbraco but not Gigya so call notifyLogin to sign in
            LoginToGigya(currentIdentity);
        }

        /// <summary>
        /// Updates the Gigya session cookie if required.
        /// </summary>
        public override void UpdateSessionExpirationCookieIfRequired(HttpContext context)
        {
            if (!_settings.EnableRaas)
            {
                _logger.Error("RaaS not enabled.");
                return;
            }

            if (_settings.SessionProvider != GigyaSessionProvider.Gigya || _settings.GigyaSessionMode != GigyaSessionMode.Sliding)
            {
                return;
            }

            var identity = ClaimsManager.GetCurrentIdentity();
            var currentIdentity = new CurrentIdentity
            {
                IsAuthenticated = identity.IsAuthenticated,
                Name = identity.Name,
                UID = identity.Name
            };

            UpdateSessionExpirationCookie(context, currentIdentity);
        }
    }
}