using Gigya.Umbraco.Module.Connector.Logging;
using Gigya.Umbraco.Module.DS.Helpers;
using Gigya.Umbraco.Module.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Umbraco.Module.DS.Helpers
{
    public static class GigyaDsHelperFactory
    {
        /// <summary>
        /// Creates a new Gigya DS helper with the settings for the current site.
        /// </summary>
        /// <returns></returns>
        public static GigyaUmbracoDsHelper Instance()
        {
            var homepageId = GigyaSettingsHelper.CurrentHomepageId();
            return Instance(homepageId);
        }

        /// <summary>
        /// Creates a new Gigya DS helper with the settings for <paramref name="siteId"/>.
        /// </summary>
        public static GigyaUmbracoDsHelper Instance(int siteId)
        {
            var logger = LoggerFactory.Instance();
            var settingsHelper = new GigyaUmbracoDsSettingsHelper(logger);
            var dsSettings = settingsHelper.Get(siteId);

            var coreSettingsHelper = new GigyaSettingsHelper();
            var coreSettings = coreSettingsHelper.Get(siteId, true);
            
            var helper = new GigyaUmbracoDsHelper(coreSettings, logger, dsSettings);
            return helper;
        }
    }
}
