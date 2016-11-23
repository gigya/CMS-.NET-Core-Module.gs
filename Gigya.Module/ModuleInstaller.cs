using Gigya.Module.Connector.Admin;
using Gigya.Module.Connector.Helpers;
using Gigya.Module.Mvc.Controllers;
using Gigya.Module.Web.Services;
using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Security.Claims;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web.Events;
using Telerik.Sitefinity;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Modules.Pages.Configuration;
using Telerik.Sitefinity.Data.Configuration;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.Connector.Logging;
using Gigya.Module.Core.Connector.Enums;
using Gigya.Module.Core.Mvc.Models;
using System.Web;

namespace Gigya.Module
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
                ModuleInstaller.RegisterModule();
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
            
            // register widgets
            restart = ModuleClass.RegisterControl("GigyaSettings", "Gigya Settings", typeof(GigyaSettingsController), "PageControls", ModuleClass.WidgetSectionName, "Telerik.Sitefinity.Mvc.Proxy.MvcControllerProxy") || restart;
            restart = ModuleClass.RegisterControl("GigyaLogin", "Gigya Login", typeof(GigyaLoginController), "PageControls", ModuleClass.WidgetSectionName, "Gigya.Module.Mvc.Proxy.MvcControllerProxyNoCache") || restart;
            restart = ModuleClass.RegisterControl("GigyaLogout", "Gigya Logout", typeof(GigyaLogoutController), "PageControls", ModuleClass.WidgetSectionName, "Gigya.Module.Mvc.Proxy.MvcControllerProxyNoCache") || restart;
            restart = ModuleClass.RegisterControl("GigyaRegister", "Gigya Register", typeof(GigyaRegisterController), "PageControls", ModuleClass.WidgetSectionName, "Gigya.Module.Mvc.Proxy.MvcControllerProxyNoCache") || restart;
            restart = ModuleClass.RegisterControl("GigyaEditProfile", "Gigya Edit Profile", typeof(GigyaEditProfileController), "PageControls", ModuleClass.WidgetSectionName, "Gigya.Module.Mvc.Proxy.MvcControllerProxyNoCache") || restart;

            if (restart)
            {
                SystemManager.RestartApplication(OperationReason.KnownKeys.DynamicModuleInstall);
            }

            // custom WCF service installation
            SystemManager.RegisterWebService(typeof(GigyaSettingsService), GigyaSettingsService.WebServiceUrl);
            
            RouteCollectionExtensions.MapRoute(RouteTable.Routes,
                "GigyaRoutes",
                "api/gigya/{controller}/{action}/{id}",
                new
                {
                    controller = "Account",
                    action = "Login",
                    id = UrlParameter.Optional
                },
                new string[] { "Gigya.Module.Mvc.Controllers" }
            );

            ObjectFactory.Container.RegisterType<GigyaMembershipHelper, GigyaMembershipHelper>("GigyaMembershipHelper",
                        new ContainerControlledLifetimeManager());

            EventHub.Subscribe<IPagePreRenderCompleteEvent>(OnPagePreRenderCompleteEventHandler);
        }

        private static void OnPagePreRenderCompleteEventHandler(IPagePreRenderCompleteEvent e)
        {
            if (e.PageSiteNode.IsBackend)
            {
                return;
            }
            
            // check if Sitefinity is the session leader and sign in if required
            GigyaAccountHelper.ValidateAndLoginToGigyaIfRequired(HttpContext.Current);
        }

        #endregion
    }
}
