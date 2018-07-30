using System;
using System.Linq;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Localization.Data;

namespace Gigya.Sitefinity.Module.DeleteSync
{
    /// <summary>
    /// Localizable strings for the Module module
    /// </summary>
    /// <remarks>
    /// You can use Sitefinity Thunder to edit this file.
    /// To do this, open the file's context menu and select Edit with Thunder.
    ///  
    /// If you wish to install this as a part of a custom module,
    /// add this to the module's Initialize method:
    /// App.WorkWith()
    ///     .Module(ModuleName)
    ///     .Initialize()
    ///         .Localization<ModuleResources>();
    /// </remarks>
    /// <see cref="http://www.sitefinity.com/documentation/documentationarticles/developers-guide/how-to/how-to-import-events-from-facebook/creating-the-resources-class"/>
    [ObjectInfo("GigyaResources", ResourceClassId = "GigyaResources", Title = "GigyaResource", TitlePlural = "GigyaResources", Description = "GigyaResources")]
    public class ModuleResources : Resource
    {
        #region Construction
        /// <summary>
        /// Initializes new instance of <see cref="ModuleResources"/> class with the default <see cref="ResourceDataProvider"/>.
        /// </summary>
        public ModuleResources()
        {
        }

        /// <summary>
        /// Initializes new instance of <see cref="ModuleResources"/> class with the provided <see cref="ResourceDataProvider"/>.
        /// </summary>
        /// <param name="dataProvider"><see cref="ResourceDataProvider"/></param>
        public ModuleResources(ResourceDataProvider dataProvider) : base(dataProvider)
        {
        }
        #endregion

        #region Class Description
        /// <summary>
        /// Module Resources
        /// </summary>
        [ResourceEntry("ModuleResourcesTitle",
            Value = "Module module labels",
            Description = "The title of this class.",
            LastModified = "2016/05/12")]
        public string ModuleResourcesTitle
        {
            get
            {
                return this["ModuleResourcesTitle"];
            }
        }

        /// <summary>
        /// Module Resources Title plural
        /// </summary>
        [ResourceEntry("ModuleResourcesTitlePlural",
            Value = "Module module labels",
            Description = "The title plural of this class.",
            LastModified = "2016/05/12")]
        public string ModuleResourcesTitlePlural
        {
            get
            {
                return this["ModuleResourcesTitlePlural"];
            }
        }

        /// <summary>
        /// Contains localizable resources for Module module.
        /// </summary>
        [ResourceEntry("ModuleResourcesDescription",
            Value = "Contains localizable resources for Module module.",
            Description = "The description of this class.",
            LastModified = "2016/05/12")]
        public string ModuleResourcesDescription
        {
            get
            {
                return this["ModuleResourcesDescription"];
            }
        }

        /// <summary>
        /// Shown to the user if a Gigya module error occurred.
        /// </summary>
        /// <value>Sorry an error occurred. Please try again.</value>
        [ResourceEntry("ErrorMessage",
            Value = "Sorry an error occurred. Please try again.",
            Description = "Shown to the user if a Gigya module error occurred.",
            LastModified = "2016/05/26")]
        public string ErrorMessage
        {
            get
            {
                return this["ErrorMessage"];
            }
        }
        #endregion

        [ResourceEntry("RedirectPageLabel",
            Value = "Redirect Page (after login)",
            Description = "",
            LastModified = "2016/05/31")]
        public string RedirectPageLabel
        {
            get
            {
                return this["RedirectPageLabel"];
            }
        }

        [ResourceEntry("LogoutPageLabel",
            Value = "Logout Page (after logout)",
            Description = "",
            LastModified = "2016/05/31")]
        public string LogoutPageLabel
        {
            get
            {
                return this["LogoutPageLabel"];
            }
        }

        /// <summary>
        /// Shown on the Gigya Settings widget in edit mode.
        /// </summary>
        /// <value>There are no settings to edit.</value>
        [ResourceEntry("SettingsWidgetNothingToEdit",
            Value = "There are no settings to edit.",
            Description = "Shown on the Gigya Settings widget in edit mode.",
            LastModified = "2016/06/01")]
        public string SettingsWidgetNothingToEdit
        {
            get
            {
                return this["SettingsWidgetNothingToEdit"];
            }
        }

        /// <summary>
        /// Greeting for logged in users.
        /// </summary>
        /// <value>Hello,</value>
        [ResourceEntry("LoggedInUserGreeting",
            Value = "Hello, ",
            Description = "Greeting for logged in users.",
            LastModified = "2016/06/06")]
        public string LoggedInUserGreeting
        {
            get
            {
                return this["LoggedInUserGreeting"];
            }
        }

        /// <summary>
        /// Sitefinity category for Gigya resources
        /// </summary>
        /// <value>Gigya</value>
        [ResourceEntry("GigyaResources",
            Value = "Gigya",
            Description = "Sitefinity category for Gigya resources",
            LastModified = "2016/06/06")]
        public string GigyaResources
        {
            get
            {
                return this["GigyaResources"];
            }
        }

        [ResourceEntry("GigyaResource",
            Value = "Gigya",
            Description = "",
            LastModified = "2016/06/06")]
        public string GigyaResource
        {
            get
            {
                return this["GigyaResource"];
            }
        }
    }
}
