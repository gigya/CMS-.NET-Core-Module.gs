using System;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;
using System.Web.UI.HtmlControls;
using Sitecore.Web.UI.HtmlControls;
using Sitecore;
using System.Text.RegularExpressions;
using Gigya.Module.Core.Connector.Helpers;
using System.Web.Security;
using Gigya.Module.Core.Connector.Enums;

namespace Sitecore.Gigya.Module.Fields
{
    public class FormsSlidingAuthModeField : Edit
    {
        public FormsSlidingAuthModeField()
        {
            this.Class = "scContentControl";
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            this.ReadOnly = true;
            this.Value = (FormsAuthentication.SlidingExpiration ? GigyaSessionMode.Sliding : GigyaSessionMode.Fixed).ToString();
        }
    }
}