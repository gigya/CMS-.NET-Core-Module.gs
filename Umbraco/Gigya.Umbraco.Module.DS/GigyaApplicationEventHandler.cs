using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;
using U = Umbraco.Web;

namespace Gigya.Umbraco.Module.DS
{
    public class GigyaApplicationEventHandler : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            RouteCollectionExtensions.MapRoute(RouteTable.Routes,
                "GigyaDsSettings",
                "GigyaDsSettings/{action}/{id}",
                new
                {
                    controller = "GigyaDsSettings",
                    action = "Index",
                    id = UrlParameter.Optional
                },
                new string[] { "Gigya.Umbraco.Module.DS..Mvc.Controllers" }
            );
        }
    }
}
