using Gigya.Module.Core.Connector.Enums;
using Gigya.Module.Core.Connector.Helpers;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.Core.Data;
using Gigya.Module.Core.Mvc.Models;
using Sitecore.Gigya.Module.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using SC = Sitecore;

namespace Sitecore.Gigya.Module.Helpers
{
    public class GigyaAccountHelper : GigyaAccountHelperBase
    {
        private readonly IAccountRepository _accountRepository;

        public GigyaAccountHelper(GigyaSettingsHelper settingsHelper, Logger logger, GigyaModuleSettings settings, IAccountRepository accountRepository) : base(settingsHelper, logger, settings)
        {
            _accountRepository = accountRepository;
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

            var currentIdentity = _accountRepository.CurrentIdentity;
            var cookieName = "glt_" + _settings.ApiKey;
            if (!currentIdentity.IsAuthenticated || HttpContext.Current.Request.Cookies[cookieName] != null)
            {
                // user not logged in
                return;
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
            
            var currentIdentity = _accountRepository.CurrentIdentity;
            UpdateSessionExpirationCookie(context, currentIdentity, isLoggingIn);
        }

        protected override void Logout()
        {
            _accountRepository.Logout();
        }
    }
}
