using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sitecore.Gigya.Extensions.Attributes
{
    public class SitecoreUserAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!Context.User.IsAuthenticated || Context.User.GetDomainName() != "sitecore")
            {
                filterContext.Result = new HttpUnauthorizedResult();
                return;
            }

            base.OnAuthorization(filterContext);
        }
    }
}