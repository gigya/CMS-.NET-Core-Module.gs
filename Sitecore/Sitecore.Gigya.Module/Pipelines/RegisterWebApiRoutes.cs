using Sitecore.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sitecore.Gigya.Module.Pipelines
{
    public class RegisterWebApiRoutes
    {
        public void Process(PipelineArgs args)
        {
            RouteTable.Routes.MapRoute("Gigya.Module.Account.Api", "api/gigya/account/{action}", new
            {
                controller = "Account"
            });
        }
    }
}