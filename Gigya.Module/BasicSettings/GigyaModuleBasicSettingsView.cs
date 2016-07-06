using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using Telerik.Sitefinity.Configuration.Web.UI.Basic;
using Telerik.Sitefinity.Web.UI;

namespace Gigya.Module.BasicSettings
{
    public class GigyaModuleBasicSettingsView : SimpleView
    {
        // The virtual path to your ascx file
        internal readonly static string layoutTemplatePath = ModuleClass.ModuleVirtualPath + "Gigya.Module.BasicSettings.GigyaModuleSettingsView.ascx";

        protected override string LayoutTemplateName
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Allows set and get of layout template path
        /// </summary>
        public override string LayoutTemplatePath
        {
            get
            {
                if(string.IsNullOrEmpty(base.LayoutTemplatePath))
                {
                    // Return virtual path to your ascx file
                    return GigyaModuleBasicSettingsView.layoutTemplatePath;
                }

                return base.LayoutTemplatePath;
            }

            set
            {
                base.LayoutTemplatePath = value;
            }
        }

        /// <summary>
        /// The Html tag that surrounds the view
        /// </summary>
        protected override System.Web.UI.HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        /// <summary>
        /// Initialize any controls for Basic Settings View
        /// </summary>
        /// <param name="container"></param>
        protected override void InitializeControls(GenericContainer container)
        {
        }
    }
}
