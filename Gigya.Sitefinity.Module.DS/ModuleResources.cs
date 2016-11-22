using System;
using System.Linq;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Localization.Data;

namespace Gigya.Sitefinity.Module
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
    }
}
