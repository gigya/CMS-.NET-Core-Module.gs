using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;
using U = Umbraco.Web;

namespace Gigya.Umbraco.Module.v621
{
    public class GigyaApplicationEventHandler : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            System.Web.Mvc.RouteCollectionExtensions.MapRoute(RouteTable.Routes,
                "GigyaRoutes",
                "api/gigya/{controller}/{action}/{id}",
                new
                {
                    controller = "Account",
                    action = "Login",
                    id = UrlParameter.Optional
                },
                new string[] { "Gigya.Umbraco.Module.v621.Mvc.Controllers" }
            );

            System.Web.Mvc.RouteCollectionExtensions.MapRoute(RouteTable.Routes,
                "GigyaSettings",
                "GigyaSettings/{action}/{id}",
                new
                {
                    controller = "GigyaSettings",
                    action = "Index",
                    id = UrlParameter.Optional
                },
                new string[] { "Gigya.Umbraco.Module.v621.Mvc.Controllers" }
            );
        }
    }
}
