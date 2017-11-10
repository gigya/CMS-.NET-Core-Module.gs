using Gigya.Module.Core.Connector.Extensions;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.Core.Data;
using Gigya.Module.Core.Mvc.Models;
using Gigya.Socialize.SDK;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Gigya.Module.Core.Connector.Helpers
{
    public abstract class GigyaAccountHelperBase
    {
        protected GigyaSettingsHelper _settingsHelper;
        protected IGigyaModuleSettings _settings;
        protected IGigyaMembershipHelper _membershipHelper;
        protected Logger _logger;

        public GigyaAccountHelperBase(GigyaSettingsHelper settingsHelper, Logger logger, IGigyaMembershipHelper membershipHelper, IGigyaModuleSettings settings = null)
        {
            _settings = settings ?? settingsHelper.GetForCurrentSite(true);
            _logger = logger;
            _settingsHelper = settingsHelper;
            _membershipHelper = membershipHelper;
        }

        public abstract void LoginToGigyaIfRequired();

        /// <summary>
        /// Logs a user into Gigya by calling NotifyLogin.
        /// </summary>
        /// <param name="currentIdentity"></param>
        protected void LoginToGigya(CurrentIdentity currentIdentity)
        {
            var uid = currentIdentity.UID;
            if (string.IsNullOrEmpty(uid))
            {
                _logger.Error("UID hasn't been set for user: " + currentIdentity.Name + " so can't login to Gigya");
                return;
            }

            var helper = new GigyaApiHelper(_settingsHelper, _logger);
            var sessionExpiration = _settingsHelper.SessionExpiration(_settings);
            var apiResponse = helper.NotifyLogin(uid, sessionExpiration, _settings);

            if (apiResponse == null)
            {
                _logger.Error("Failed to call gigya.accounts.notifyLogin for user " + currentIdentity.Name);
                return;
            }

            var notifyLoginResponse = JsonConvert.DeserializeObject<NotifyLoginResponse>(apiResponse.GetResponseText());
            if (notifyLoginResponse.sessionInfo != null)
            {
                var cookie = new HttpCookie(notifyLoginResponse.sessionInfo.cookieName, notifyLoginResponse.sessionInfo.cookieValue);
                if (sessionExpiration > 0)
                {
                    cookie.Expires = DateTime.UtcNow.AddSeconds(sessionExpiration);
                }

                HttpContext.Current.Response.Cookies.Add(cookie);
                _logger.DebugFormat("Successfully logged in user {0} using gigya.accounts.notifyLogin", currentIdentity.Name);
            }
            else
            {
                _logger.Error("No sessionInfo returned from gigya.accounts.notifyLogin for user " + currentIdentity.Name);
            }
        }

        public abstract void UpdateSessionExpirationCookieIfRequired(HttpContext context);

        protected virtual void UpdateSessionExpirationCookie(HttpContext context, CurrentIdentity currentIdentity)
        {
            if (_settings.SessionProvider != Enums.GigyaSessionProvider.Gigya || _settings.GigyaSessionMode != Enums.GigyaSessionMode.Sliding)
            {
                return;
            }

            var gigyaAuthCookie = context.Request.Cookies["glt_" + _settings.ApiKey];
            if (gigyaAuthCookie == null || string.IsNullOrEmpty(gigyaAuthCookie.Value))
            {
                if (currentIdentity.IsAuthenticated)
                {
                    // not logged into gigya so sign out of CMS
                    _membershipHelper.Logout();
                }
                return;
            }

            var cookie = new HttpCookie("gltexp_" + _settings.ApiKey);
            var sessionExpiration = _settingsHelper.SessionExpiration(_settings);
            cookie.Expires = DateTime.Now.AddSeconds(sessionExpiration);

            var gigyaAuthCookieSplit = HttpUtility.UrlDecode(gigyaAuthCookie.Value).Split('|');
            var loginToken = gigyaAuthCookieSplit[0];
            cookie.Value = GigyaSignatureHelpers.GetDynamicSessionSignatureUserSigned(loginToken, sessionExpiration, _settings.ApplicationKey, _settings.ApplicationSecret);
            cookie.Path = "/";

            context.Response.Cookies.Set(cookie);

            if (!currentIdentity.IsAuthenticated)
            {
                // try and get the user from the UID
                //UUID=b2f5dda5de6046c5ab870c7376641ab8



                _membershipHelper.Login(currentIdentity.Name);
            }
        }
    }
}
