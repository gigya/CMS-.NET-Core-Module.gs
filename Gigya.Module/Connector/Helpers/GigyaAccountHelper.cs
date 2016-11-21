using Gigya.Module.Core.Connector.Enums;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.Core.Data;
using Gigya.Module.Core.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Claims;
using Telerik.Sitefinity.Security.Model;

namespace Gigya.Module.Connector.Helpers
{
    public class GigyaAccountHelper : Gigya.Module.Core.Connector.Helpers.GigyaAccountHelperBase
    {
        public GigyaAccountHelper(GigyaSettingsHelper settingsHelper, Logger logger) : base(settingsHelper, logger)
        {
        }

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
    }
}