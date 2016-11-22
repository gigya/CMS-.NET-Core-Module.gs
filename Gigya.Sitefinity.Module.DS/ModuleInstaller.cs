using Gigya.Module.Core.Connector.Events;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.DS.Helpers;
using Gigya.Sitefinity.Module.DS.Helpers;
using Gigya.Sitefinity.Module.DS.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Data.Configuration;
using Telerik.Sitefinity.Services;

namespace Gigya.Sitefinity.Module.DS
{
    public static class ModuleInstaller
    {
        public static void PreApplicationStart()
        {
            Bootstrapper.Initialized += ModuleInstaller.OnBootstrapperInitialized;
        }

        /// <summary>
        /// Called when the Bootstrapper is initialized.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Telerik.Sitefinity.Data.ExecutedEventArgs" /> instance containing the event data.</param>
        private static void OnBootstrapperInitialized(object sender, ExecutedEventArgs e)
        {
            if (e.CommandName == "RegisterRoutes")
            {
                // We have to register the module at a very early stage when sitefinity is initializing
                ModuleInstaller.RegisterModule();
            }
        }

        private static void RegisterModule()
        {
            var restart = false;
            var configManager = ConfigManager.GetManager();

            // check if Sitefinity has been installed
            var dataconfig = configManager.GetSection<DataConfig>();
            if (dataconfig == null || !dataconfig.Initialized || dataconfig.ConnectionStrings.Count == 0)
            {
                return;
            }

            var modulesConfig = configManager.GetSection<SystemConfig>().ApplicationModules;
            if (!modulesConfig.Elements.Any(el => el.GetKey().Equals(ModuleClass.ModuleName)))
            {
                modulesConfig.Add(ModuleClass.ModuleName, new AppModuleSettings(modulesConfig)
                {
                    Name = ModuleClass.ModuleName,
                    Title = ModuleClass.ModuleTitle,
                    Description = ModuleClass.ModuleDescription,
                    Type = typeof(ModuleClass).AssemblyQualifiedName,
                    StartupType = StartupType.OnApplicationStart
                });

                configManager.SaveSection(modulesConfig.Section);

                restart = true;
            }
            
            if (restart)
            {
                SystemManager.RestartApplication(OperationReason.KnownKeys.DynamicModuleInstall);
            }

            // custom WCF service installation
            SystemManager.RegisterWebService(typeof(GigyaDSSettingsService), GigyaDSSettingsService.WebServiceUrl);

            // subscribe to get account info event so we can merge DS data
            GigyaEventHub.Instance.GetAccountInfoCompleted += GigyaEventHub_GetAccountInfoCompleted;
        }

        /// <summary>
        /// Event that is called whenever the user's data is retrieved with getAccountInfo.
        /// This method retrieves the ds data and merges it with the getAccountInfo object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void GigyaEventHub_GetAccountInfoCompleted(object sender, GetAccountInfoCompletedEventArgs e)
        {
            var settingsHelper = new GigyaSitefinityDsSettingsHelper(e.Logger);
            var settings = settingsHelper.Get((Guid)e.CurrentSiteId);

            // merge ds data with account info
            var helper = new GigyaDsHelper(e.Settings, e.Logger, settings);
            e.GigyaModel = helper.Merge(e.GigyaModel, e.MappingFields);
        }
    }
}
