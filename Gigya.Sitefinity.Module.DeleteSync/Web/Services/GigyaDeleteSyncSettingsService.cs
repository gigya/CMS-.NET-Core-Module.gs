using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.SiteSettings;
using Telerik.Sitefinity.SiteSettings.Web.Services;
using Telerik.Sitefinity.Web.Services;
using Gigya.Module.Connector.Logging;
using Gigya.Sitefinity.Module.DeleteSync.BasicSettings;
using Telerik.Sitefinity.Utilities.TypeConverters;

namespace Gigya.Sitefinity.Module.DeleteSync.Web.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single)]
    public class GigyaDeleteSyncSettingsService : IBasicSettingsService
    {
        internal const string WebServiceUrl = "Sitefinity/CustomServices/GigyaDeleteSyncSettings.svc";
        
        /// <summary>
        /// Called by the client to retrieve the settings.
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public SettingsItemContext GetSettings(string itemType, string siteId)
        {
            ServiceUtility.RequestBackendUserAuthentication();

            if (itemType == null)
            {
                throw new ArgumentNullException("itemType");
            }

            Type type = TypeResolutionService.ResolveType(itemType);
            if (!typeof(IGigyaSettingsDataContract).IsAssignableFrom(type))
            {
                throw new Exception("The settings type specified by 'itemType' parameter must implement 'IGigyaDSSettingsDataContract' interface");
            }

            Guid id = Guid.Empty;
            if (SystemManager.CurrentContext.IsMultisiteMode && !string.IsNullOrEmpty(siteId))
            {
                id = Guid.Parse(siteId);
            }
            
            var settingsDataContract = (IGigyaSettingsDataContract)Activator.CreateInstance(type);
            settingsDataContract.Load();

            return new SettingsItemContext(settingsDataContract, true);
        }

        /// <summary>
        /// Called by the client to save the updated settings.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <param name="itemType"></param>
        /// <param name="siteId"></param>
        /// <param name="inheritanceState"></param>
        public void SaveSettings(SettingsItemContext context, string key, string itemType, string siteId, string inheritanceState)
        {
            ServiceUtility.RequestBackendUserAuthentication();

            if (itemType == null)
            {
                throw new ArgumentNullException("itemType");
            }

            Guid id = Guid.Empty;
            if (!string.IsNullOrEmpty(siteId))
            {
                id = Guid.Parse(siteId);
            }

            IGigyaSettingsDataContract settingsDataContract = (IGigyaSettingsDataContract)context.Item;
            settingsDataContract.Save();
        }
    }
}
