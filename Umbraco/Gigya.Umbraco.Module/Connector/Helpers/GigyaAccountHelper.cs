using Gigya.Module.Core.Connector.Enums;
using Gigya.Module.Core.Connector.Helpers;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.Core.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbraco.Core;

namespace Gigya.Umbraco.Module.Connector.Helpers
{
    public class GigyaAccountHelper : Gigya.Module.Core.Connector.Helpers.GigyaAccountHelperBase
    {
        public GigyaAccountHelper(GigyaSettingsHelper settingsHelper, Logger logger, IGigyaMembershipHelper membershipHelper) : base(settingsHelper, logger, membershipHelper)
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

            var context = HttpContext.Current;
            var currentIdentity = new CurrentIdentity
            {
                IsAuthenticated = context.User.Identity.IsAuthenticated,
                Name = context.User.Identity.Name
            };

            var cookieName = "glt_" + _settings.ApiKey;
            if (!currentIdentity.IsAuthenticated || HttpContext.Current.Request.Cookies[cookieName] != null)
            {
                // user not logged in
                return;
            }

            // get UID if not the username
            var uidMapping = _settings.MappedMappingFields.FirstOrDefault(i => i.GigyaFieldName == Constants.GigyaFields.UserId && !string.IsNullOrEmpty(i.CmsFieldName));
            if (uidMapping != null && uidMapping.CmsFieldName != Constants.CmsFields.Username)
            {
                // get member to find UID field
                var member = ApplicationContext.Current.Services.MemberService.GetByUsername(currentIdentity.Name);
                if (member == null)
                {
                    _logger.Error(string.Format("Couldn't find member with username of {0} so couldn't sign them in.", currentIdentity.Name));
                    return;
                }

                currentIdentity.UID = member.GetValue<string>(uidMapping.CmsFieldName);
            }
            
            // user logged into Umbraco but not Gigya so call notifyLogin to sign in
            LoginToGigya(currentIdentity);
        }

        /// <summary>
        /// Updates the Gigya session cookie if required.
        /// </summary>
        public override void UpdateSessionExpirationCookieIfRequired(HttpContext context, bool isLoggingIn = false)
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
            
            var currentIdentity = new CurrentIdentity
            {
                IsAuthenticated = context.User.Identity.IsAuthenticated,
                Name = context.User.Identity.Name
            };

            // get UID if not the username
            var uidMapping = _settings.MappedMappingFields.FirstOrDefault(i => i.GigyaFieldName == Constants.GigyaFields.UserId && !string.IsNullOrEmpty(i.CmsFieldName));
            if (uidMapping != null && uidMapping.CmsFieldName != Constants.CmsFields.Username)
            {
                // get member to find UID field
                var member = ApplicationContext.Current.Services.MemberService.GetByUsername(currentIdentity.Name);
                if (member == null)
                {
                    _logger.Error(string.Format("Couldn't find member with username of {0} so couldn't sign them in.", currentIdentity.Name));
                    return;
                }

                currentIdentity.UID = member.GetValue<string>(uidMapping.CmsFieldName);
            }

            UpdateSessionExpirationCookie(context, currentIdentity, isLoggingIn);
        }
    }
}
