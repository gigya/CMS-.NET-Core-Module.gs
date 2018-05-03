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
        protected Logger _logger;

        public GigyaAccountHelperBase(GigyaSettingsHelper settingsHelper, Logger logger, IGigyaModuleSettings settings = null)
        {
            _settings = settings ?? settingsHelper.GetForCurrentSite(true);
            _logger = logger;
            _settingsHelper = settingsHelper;
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

        private bool IsDynamicSessionExtensionRequired(HttpContext context, bool isLoggingIn)
        {
            if (_settings.SessionProvider != Enums.GigyaSessionProvider.Gigya || _settings.GigyaSessionMode != Enums.GigyaSessionMode.Sliding)
            {
                return false;
            }

            if (isLoggingIn)
            {
                return true;
            }

            var gigyaExpCookie = context.Request.Cookies["gltexp_" + _settings.ApiKey];
            if (gigyaExpCookie == null)
            {
                return false;
            }

            var currentSessionExpiryEpoch = 0L;
            var gigyaAuthCookieSplit = HttpUtility.UrlDecode(gigyaExpCookie.Value).Split('_');
            if (gigyaAuthCookieSplit.Length == 0 || !long.TryParse(gigyaAuthCookieSplit[0], out currentSessionExpiryEpoch))
            {
                // no cookie provided so we need to create one...according to Inbal's sample code
                return true;
            }

            var epoch = DateTime.UtcNow.DateTimeToUnixTimestamp();
            return currentSessionExpiryEpoch > epoch;
        }

        protected abstract void Logout();

        public abstract void UpdateSessionExpirationCookieIfRequired(HttpContext context, bool isLoggingIn = false);
        
        protected virtual void UpdateSessionExpirationCookie(HttpContext context, CurrentIdentity currentIdentity, bool isLoggingIn)
        {
            if (!IsDynamicSessionExtensionRequired(context, isLoggingIn))
            {
                return;
            }

            var gigyaAuthCookie = context.Request.Cookies["glt_" + _settings.ApiKey];
            if (gigyaAuthCookie == null || string.IsNullOrEmpty(gigyaAuthCookie.Value))
            {
                if (currentIdentity.IsAuthenticated)
                {
                    // not logged into gigya so sign out of CMS
                    Logout();
                }
                return;
            }

            var cookie = new HttpCookie("gltexp_" + _settings.ApiKey);
            var sessionExpiration = _settingsHelper.SessionExpiration(_settings);
            cookie.Expires = DateTime.UtcNow.AddYears(10);

            var gigyaAuthCookieSplit = HttpUtility.UrlDecode(gigyaAuthCookie.Value).Split('|');
            var loginToken = gigyaAuthCookieSplit[0];
            cookie.Value = GigyaSignatureHelpers.GetDynamicSessionSignatureUserSigned(loginToken, sessionExpiration, _settings.ApplicationKey, _settings.ApplicationSecret);
            cookie.Path = "/";
            context.Response.Cookies.Set(cookie);
        }
    }
}
