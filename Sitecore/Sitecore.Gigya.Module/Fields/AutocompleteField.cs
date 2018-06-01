using System;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;
using System.Web.UI.HtmlControls;
using Sitecore.Web.UI.HtmlControls;
using Sitecore;
using System.Text.RegularExpressions;
using Gigya.Module.Core.Connector.Helpers;
using System.Web.UI;

namespace Sitecore.Gigya.Module.Fields
{
    public class AutocompleteField : Sitecore.Shell.Applications.ContentEditor.Text
    {
        public AutocompleteField()
        {
            this.Class = "scContentControl gigya-autocomplete";
        }

        public string ItemID { get; set; }

        protected override void DoRender(HtmlTextWriter output)
        {
            base.DoRender(output);

            output.Write(@"<script>
                        $sc(function () {
                            $sc('#" + this.ClientID + @"').autocomplete({
                                serviceUrl: '/sitecore/shell/gigya/contenteditor/results/" + ItemID + @"'
                            });
                        });
                    </script>");
        }
    }
}