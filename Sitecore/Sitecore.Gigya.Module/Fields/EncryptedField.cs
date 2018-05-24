using System;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;
using System.Web.UI.HtmlControls;
using Sitecore.Web.UI.HtmlControls;
using Sitecore;
using System.Text.RegularExpressions;
using Gigya.Module.Core.Connector.Helpers;

namespace Sitecore.Gigya.Module.Fields
{
    public class EncryptedField : Edit
    {
        public EncryptedField()
        {
            this.Class = "scContentControl";
        }

        private bool IsCurrentUserAllowedToEditAndView()
        {
            var user = Sitecore.Context.User;
            if (user == null)
            {
                return false;
            }

            return user.IsAdministrator || user.IsInRole(Constants.Security.AdminRole);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var value = this.ServerProperties["Value"] as string;

            // save a copy of the original value on the server
            this.ServerProperties["OriginalValue"] = value;

            if (!IsCurrentUserAllowedToEditAndView())
            {
                // not allowed to view or edit so clear the value that is displayed
                this.ServerProperties["Value"] = this.Value = string.Empty;
                this.ReadOnly = true;
                return;
            }

            MaskEncryptedValue(value);
        }

        private void MaskEncryptedValue(string value)
        {
            var settingsHelper = new Helpers.GigyaSettingsHelper();
            var plainTextApplicationSecret = settingsHelper.TryDecryptApplicationSecret(value, false);
            if (string.IsNullOrEmpty(plainTextApplicationSecret))
            {
                return;
            }

            value = StringHelper.MaskInput(plainTextApplicationSecret, "*", 2, 2);
            this.ServerProperties["PlainTextValue"] = value;
            this.Value = value;
        }

        protected override bool LoadPostData(string value)
        {
            value = StringUtil.GetString(new string[1]
            {
                value
            });

            var plainTextValue = this.ServerProperties["PlainTextValue"] as string;
            if (this.ReadOnly || value == plainTextValue)
            {
                // field is read only or value is unchanged
                var originalValue = this.ServerProperties["OriginalValue"] as string;
                if (!string.IsNullOrEmpty(originalValue))
                {
                    this.Value = originalValue;
                }
                return false;
            }

            if (!(this.Value != value))
            {
                return false;
            }

            this.Value = value;

            base.SetModified();

            return true;
        }
    }
}