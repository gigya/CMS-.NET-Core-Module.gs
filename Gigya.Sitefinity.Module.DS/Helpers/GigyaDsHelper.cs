using Gigya.Module.Connector.Helpers;
using Gigya.Module.Connector.Logging;
using Gigya.Module.DS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Sitefinity.Module.DS.Helpers
{
    public static class GigyaDsHelperFactory
    {
        /// <summary>
        /// Creates a new Gigya DS helper with the settings for <paramref name="siteId"/>.
        /// </summary>
        public static GigyaDsHelper Instance(Guid siteId)
        {
            var logger = LoggerFactory.Instance();
            var settingsHelper = new GigyaSitefinityDsSettingsHelper(logger);
            var dsSettings = settingsHelper.Get(siteId);

            var coreSettingsHelper = new GigyaSettingsHelper();
            var coreSettings = coreSettingsHelper.Get(siteId, true);

            // merge ds data with account info
            var helper = new GigyaDsHelper(coreSettings, logger, dsSettings);
            return helper;
        }
    }
}
