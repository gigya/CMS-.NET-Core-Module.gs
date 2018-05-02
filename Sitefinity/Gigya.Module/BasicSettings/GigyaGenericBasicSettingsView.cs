using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Sitefinity.Configuration.Web.UI.Basic;
using Telerik.Sitefinity.SiteSettings;
using Telerik.Sitefinity.Web.UI;

namespace Gigya.Module.BasicSettings
{
    public class GigyaGenericBasicSettingsView<TFieldsView, TDataContract> : BasicSettingsView where TFieldsView : Control, new() where TDataContract : ISettingsDataContract
    {
        public static readonly string layoutTemplateName = ModuleClass.ModuleVirtualPath + "Gigya.Module.BasicSettings.GigyaGenericBasicSettingsView.ascx";

        public Type DataContract { get; set; }

        protected override string LayoutTemplateName
        {
            get
            {
                return (string)null;
            }
        }

        public override string LayoutTemplatePath
        {
            get
            {
                if (string.IsNullOrEmpty(base.LayoutTemplatePath))
                    return GenericBasicSettingsView<TFieldsView, TDataContract>.layoutTemplateName;
                return base.LayoutTemplatePath;
            }
            set
            {
                base.LayoutTemplatePath = value;
            }
        }

        public virtual PlaceHolder FieldsContainer
        {
            get
            {
                return this.Container.GetControl<PlaceHolder>("fieldsContainer", true);
            }
        }

        public GigyaGenericBasicSettingsView()
        {
            this.DataContract = typeof(TDataContract);
        }

        protected override void InitializeControls(Control viewContainer)
        {
            base.InitializeControls(viewContainer);
            if (this.DataContract == (Type)null)
                throw new Exception("Required property 'SettingsDataContract' is not provided");
            this.FieldsBinder.ServiceUrl = string.Format("~/Sitefinity/CustomServices/GigyaSettings.svc/generic/?itemType={0}", (object)HttpUtility.UrlEncode(this.DataContract.FullName));
            this.FieldsContainer.Controls.Add((Control)Activator.CreateInstance<TFieldsView>());
        }
    }
}
