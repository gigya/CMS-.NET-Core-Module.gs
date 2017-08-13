using Gigya.Sitefinity.Module.DS.BasicSettings;
using Gigya.Sitefinity.Module.DS.Configuration;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Sitefinity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Abstractions.VirtualPath.Configuration;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Configuration.Web.UI.Basic;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Fluent.Modules;
using Telerik.Sitefinity.Fluent.Modules.Toolboxes;
using Telerik.Sitefinity.Modules.Pages.Configuration;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web.UI;

namespace Gigya.Sitefinity.Module.DS
{
    /// <summary>
    /// Custom Sitefinity module 
    /// </summary>
    public class ModuleClass : ModuleBase
    {
        #region Properties
        /// <summary>
        /// Gets the landing page id for the module.
        /// </summary>
        /// <value>The landing page id.</value>
        public override Guid LandingPageId
        {
            get
            {
                return SiteInitializer.DashboardPageNodeId;
            }
        }

        /// <summary>
        /// Gets the CLR types of all data managers provided by this module.
        /// </summary>
        /// <value>An array of <see cref="T:System.Type" /> objects.</value>
        public override Type[] Managers
        {
            get
            {
                return new Type[0];
            }
        }
        #endregion

        #region Module Initialization
        /// <summary>
        /// Initializes the service with specified settings.
        /// This method is called every time the module is initializing (on application startup by default)
        /// </summary>
        /// <param name="settings">The settings.</param>
        public override void Initialize(ModuleSettings settings)
        {
            base.Initialize(settings);

            // Add your initialization logic here
            // here we register the module resources
            // but if you have you should register your module configuration or web service here

            App.WorkWith()
                .Module(settings.Name)
                    .Initialize()
                    .Localization<ModuleResources>()
                    .Configuration<GigyaDSModuleConfig>()
                    .BasicSettings<GigyaDSGenericBasicSettingsView<GigyaDSModuleBasicSettingsView, GigyaDSModuleSettingsContract>>
                        ("GigyaDSModule", "Gigya DS", "", typeof(GigyaDSModuleSettingsContract), true);



            //App.WorkWith()
            //    .Module(settings.Name)
            //        .Initialize()
            //        .Localization<ModuleResources>();

            // Here is also the place to register to some Sitefinity specific events like Bootstrapper.Initialized or subscribe for an event in with the EventHub class            
            // Please refer to the documentation for additional information http://www.sitefinity.com/documentation/documentationarticles/developers-guide/deep-dive/sitefinity-event-system/ieventservice-and-eventhub
        }

        /// <summary>
        /// Installs this module in Sitefinity system for the first time.
        /// </summary>
        /// <param name="initializer">The Site Initializer. A helper class for installing Sitefinity modules.</param>
        public override void Install(SiteInitializer initializer)
        {
            // Here you can install a virtual path to be used for this assembly
            // A virtual path is required to access the embedded resources
            this.InstallVirtualPaths(initializer);

            // Here you can install you backend pages
            //this.InstallBackendPages(initializer);

            // Here you can also install your page/form/layout widgets
            this.InstallPageWidgets(initializer);
        }

        /// <summary>
        /// Upgrades this module from the specified version.
        /// This method is called instead of the Install method when the module is already installed with a previous version.
        /// </summary>
        /// <param name="initializer">The Site Initializer. A helper class for installing Sitefinity modules.</param>
        /// <param name="upgradeFrom">The version this module us upgrading from.</param>
        public override void Upgrade(SiteInitializer initializer, Version upgradeFrom)
        {
            // Here you can check which one is your prevous module version and execute some code if you need to
            // See the example bolow
            //
            //if (upgradeFrom < new Version("1.0.1.0"))
            //{
            //    some upgrade code that your new version requires
            //}
        }

        /// <summary>
        /// Uninstalls the specified initializer.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        public override void Uninstall(SiteInitializer initializer)
        {
            base.Uninstall(initializer);
            // Add your uninstall logic here
        }
        #endregion

        #region Public and overriden methods
        /// <summary>
        /// Gets the module configuration.
        /// </summary>
        protected override ConfigSection GetModuleConfig()
        {
            // If you have a module configuration, you should return it here
            // return Config.Get<ModuleConfigurationType>();
            return null;
        }
        #endregion

        #region Virtual paths
        /// <summary>
        /// Installs module virtual paths.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        private void InstallVirtualPaths(SiteInitializer initializer)
        {
            // Here you can register your module virtual paths

            var virtualPaths = initializer.Context.GetConfig<VirtualPathSettingsConfig>().VirtualPaths;
            var moduleVirtualPath = ModuleClass.ModuleVirtualPath + "*";
            if (!virtualPaths.ContainsKey(moduleVirtualPath))
            {
                virtualPaths.Add(new VirtualPathElement(virtualPaths)
                {
                    VirtualPath = moduleVirtualPath,
                    ResolverName = "EmbeddedResourceResolver",
                    ResourceLocation = typeof(ModuleClass).Assembly.GetName().Name
                });
            }
        }
        #endregion

        #region Install backend pages
        /// <summary>
        /// Installs the backend pages.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        private void InstallBackendPages(SiteInitializer initializer)
        {
            // Using our Fluent Api you can add your module backend pages hierarchy in no time
            // Here is an example using resources to localize the title of the page and adding a dummy control
            // to the module page. 

            //Guid groupPageId = new Guid("d071494f-5150-4ff1-8320-589d73955a4c");
            //Guid pageId = new Guid("592476a6-0fd0-4e59-b042-eb20766ef601");

            //initializer.Installer
            //    .CreateModuleGroupPage(groupPageId, "Module group page")
            //        .PlaceUnder(SiteInitializer.SitefinityNodeId)
            //        .SetOrdinal(100)
            //        .LocalizeUsing<ModuleResources>()
            //        .SetTitleLocalized("ModuleGroupPageTitle")
            //        .SetUrlNameLocalized("ModuleGroupPageUrlName")
            //        .SetDescriptionLocalized("ModuleGroupPageDescription")
            //        .ShowInNavigation()
            //        .AddChildPage(pageId, "Module page")
            //            .SetOrdinal(1)
            //            .LocalizeUsing<ModuleResources>()
            //            .SetTitleLocalized("ModulePageTitle")
            //            .SetHtmlTitleLocalized("ModulePageTitle")
            //            .SetUrlNameLocalized("ModulePageUrlName")
            //            .SetDescriptionLocalized("ModulePageDescription")
            //            .AddControl(new System.Web.UI.WebControls.Literal()
            //            {
            //                Text = "<h1 class=\"sfBreadCrumb\">Module module Installed</h1>",
            //                Mode = LiteralMode.PassThrough
            //            })
            //            .ShowInNavigation()
            //        .Done()
            //    .Done();
        }
        #endregion

        #region Widgets
        /// <summary>
        /// Installs the form widgets.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        private void InstallFormWidgets(SiteInitializer initializer)
        {
            // Here you can register your custom form widgets in the toolbox.
            // See the example below.

            //string moduleFormWidgetSectionName = "Module";
            //string moduleFormWidgetSectionTitle = "Module";
            //string moduleFormWidgetSectionDescription = "Module";

            //initializer.Installer
            //    .Toolbox(CommonToolbox.FormWidgets)
            //        .LoadOrAddSection(moduleFormWidgetSectionName)
            //            .SetTitle(moduleFormWidgetSectionTitle)
            //            .SetDescription(moduleFormWidgetSectionDescription)
            //            .LoadOrAddWidget<WidgetType>(WidgetNameForDevelopers)
            //                .SetTitle(WidgetTitle)
            //                .SetDescription(WidgetDescription)
            //                .LocalizeUsing<ModuleResources>()
            //                .SetCssClass(WidgetCssClass) // You can use a css class to add an icon (this is optional)
            //            .Done()
            //        .Done()
            //    .Done();
        }

        /// <summary>
        /// Installs the layout widgets.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        private void InstallLayoutWidgets(SiteInitializer initializer)
        {
            // Here you can register your custom layout widgets in the toolbox.
            // See the example below.

            //string moduleLayoutWidgetSectionName = "Module";
            //string moduleLayoutWidgetSectionTitle = "Module";
            //string moduleLayoutWidgetSectionDescription = "Module";

            //initializer.Installer
            //    .Toolbox(CommonToolbox.Layouts)
            //        .LoadOrAddSection(moduleLayoutWidgetSectionName)
            //            .SetTitle(moduleLayoutWidgetSectionTitle)
            //            .SetDescription(moduleLayoutWidgetSectionDescription)
            //            .LoadOrAddWidget<LayoutControl>(WidgetNameForDevelopers)
            //                .SetTitle(WidgetTitle)
            //                .SetDescription(WidgetDescription)
            //                .LocalizeUsing<ModuleResources>()
            //                .SetCssClass(WidgetCssClass) // You can use a css class to add an icon (Optional)
            //                .SetParameters(new NameValueCollection() 
            //                { 
            //                    { "layoutTemplate", FullPathToTheLayoutWidget },
            //                })
            //            .Done()
            //        .Done()
            //    .Done();
        }

        /// <summary>
        /// Installs the page widgets.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        private void InstallPageWidgets(SiteInitializer initializer)
        {
            // Here you can register your custom page widgets in the toolbox.
            // See the example below.

            //string modulePageWidgetSectionName = "Gigya";
            //string modulePageWidgetSectionTitle = "Gigya";
            //string modulePageWidgetSectionDescription = "Gigya Widgets";

            //initializer.Installer
            //    .Toolbox(CommonToolbox.PageWidgets)
            //        .LoadOrAddSection(modulePageWidgetSectionName)
            //            .SetTitle(modulePageWidgetSectionTitle)
            //            .SetDescription(modulePageWidgetSectionDescription)
            //            .LoadOrAddWidget<GigyaSettingsController>("Gigya Settings")
            //                .SetTitle("WidgetTitle")
            //                .SetDescription("WidgetDescription")
            //                .LocalizeUsing<ModuleResources>()
            //                .SetCssClass("WidgetCssClass") // You can use a css class to add an icon (Optional)
            //            .Done()
            //        .Done()
            //    .Done();
        }

        #endregion

        #region Upgrade methods
        #endregion

        #region Private members & constants
        public const string Version = "1.0.0.6";
        public const string AssemblyName = "Gigya.Sitefinity.Module.DS";
        public const string ModuleName = "Gigya DS Module";
        internal const string ModuleTitle = "Gigya DS Module";
        internal const string ModuleDescription = "This is a Custom Module which has been built to integrate with Gigya's DS service.";
        internal const string ModuleVirtualPath = "~/GigyaDSModule/";
        internal const string WidgetSectionName = "Gigya";
        #endregion
    }
}