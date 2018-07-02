using Sitecore.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sitecore.Gigya.Connector.Pipelines
{
    public class RegisterWebApiRoutes
    {
        public void Process(PipelineArgs args)
        {
            RouteTable.Routes.MapRoute("Gigya.Module.Account.Api", "api/gigya/{controller}/{action}/{id}", new
            {
                controller = "Account",
                id = UrlParameter.Optional
            });

            RouteTable.Routes.MapRoute("Gigya.Module.ContentEditor.Api", "sitecore/shell/gigya/contenteditor/{action}/{id}", new
            {
                controller = "ContentEditor",
                id = UrlParameter.Optional
            });
        }
    }
}