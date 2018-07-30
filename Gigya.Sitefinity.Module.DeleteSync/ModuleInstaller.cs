using System.Linq;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Data.Configuration;
using Gigya.Sitefinity.Module.DeleteSync.Web.Services;
using System;

namespace Gigya.Sitefinity.Module.DeleteSync
{
    /// <summary>
    /// Module installer class
    /// </summary>
    /// <remarks>
    /// This installer is registered in the /Properties/AssemblyInfo.cs file
    /// The purpose of it is to register the module in Sitefinity automatically.
    /// The User will have to enable the module from Administration -> Modules & Services
    /// </remarks>
    /// <see cref="http://www.sitefinity.com/blogs/peter-marinovs-blog/2013/03/20/creating-self-installing-widgets-and-modules-in-sitefinity"/>
    public static class ModuleInstaller
    {
        #region Public methods
        /// <summary>
        /// Called before the application start.
        /// </summary>
        public static void PreApplicationStart()
        {
            Bootstrapper.Initialized += ModuleInstaller.OnBootstrapperInitialized;
        }
        #endregion

        #region Private methods
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
                RegisterModule();
            }
        }

        /// <summary>
        /// Registers the Module module.
        /// </summary>
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

            try
            {
                SystemManager.RegisterWebService(typeof(GigyaDeleteSyncSettingsService), GigyaDeleteSyncSettingsService.WebServiceUrl);
            }
            catch (Exception e)
            {

            }
        }

        #endregion
    }
}
