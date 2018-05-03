using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.Core.Data;
using Gigya.Module.DS.Config;
using Gigya.Module.DS.Helpers;
using Gigya.Umbraco.Module.Connector.Helpers;
using Gigya.Umbraco.Module.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Umbraco.Module.DS.Helpers
{
    public class GigyaUmbracoDsHelper : GigyaDsHelper
    {
        public GigyaUmbracoDsHelper(IGigyaModuleSettings settings, Logger logger, GigyaDsSettings dsSettings, GigyaDsApiHelper apiHelper = null) : base(settings, logger, dsSettings, apiHelper)
        {
        }

        /// <summary>
        /// Fetches ds data using the configured method (get or search) for the current logged in user.
        /// If the user is not logged in, null will be returned.
        /// After completing the DS call, the FetchDSCompleted event will be fired.
        /// </summary>
        public dynamic GetOrSearchForCurrentUser()
        {
            var settingsHelper = new GigyaSettingsHelper();
            var apiHelper = new Gigya.Module.Core.Connector.Helpers.GigyaApiHelper(settingsHelper, _logger);
            var accountHelper = new GigyaAccountHelper(settingsHelper, _logger);

            var membershipHelper = new GigyaMembershipHelper(apiHelper, accountHelper, _logger);
            var currentUid = membershipHelper.GetUidForCurrentUser(_settings);

            return GetOrSearch(currentUid);
        }
    }
}