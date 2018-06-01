using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.StringExtensions;
//using Sitecore.Web.UI.HtmlControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Sitecore.Gigya.Module.Pipelines
{
    public class GigyaContentEditorPipeline
    {
        public void Process(PipelineArgs args)
        {
            if (!Context.ClientPage.IsEvent)
            {
                HttpContext current = HttpContext.Current;
                if (current != null)
                {
                    Page handler = current.Handler as Page;
                    if (handler != null)
                    {
                        Assert.IsNotNull(handler.Header, "Content Editor <head> tag is missing runat='value'");
                        handler.Header.Controls.Add(new LiteralControl("<link rel=\"stylesheet\" href=\"/sitecore modules/gigya/css/gigya-content-editor.css\" />"));
                        handler.Header.Controls.Add(new LiteralControl("<script src=\"/scripts/gigya/jQuery-Autocomplete/jquery.autocomplete.min.js\"></script>"));
                    }
                }
            }
        }
    }
}