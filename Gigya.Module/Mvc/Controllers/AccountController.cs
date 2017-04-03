using System;
using System.Linq;
using System.Web.Mvc;
using Telerik.Sitefinity.Security;
using Gigya.Module.Connector.Helpers;
using Telerik.Sitefinity.Security.Claims;
using Gigya.Module.Core.Mvc.Models;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.Connector.Logging;
using Gigya.Module.Core.Connector.Helpers;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Security.Configuration;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System.Web;

namespace Gigya.Module.Mvc.Controllers
{
    public class AccountController : Gigya.Module.Core.Mvc.Controllers.AccountController
    {
        public AccountController() : base()
        {
            Logger = new Logger(new SitefinityLogger());
            SettingsHelper = new Connector.Helpers.GigyaSettingsHelper();
            var apiHelper = new GigyaApiHelper(SettingsHelper, Logger);
            MembershipHelper = new GigyaMembershipHelper(apiHelper, Logger);
        }

        protected override CurrentIdentity GetCurrentIdentity()
        {
            var currentIdentity = ClaimsManager.GetCurrentIdentity();
            return new CurrentIdentity
            {
                IsAuthenticated = currentIdentity.IsAuthenticated,
                Name = currentIdentity.Name
            };
        }

        public override ActionResult Login(LoginModel model)
        {
            if (SystemManager.IsDesignMode || SystemManager.IsPreviewMode)
            {
                // can't login in design or preview mode
                return JsonNetResult(new LoginResponseModel());
            }

            return base.Login(model);
        }

        protected override void Signout()
        {
            if (Config.Get<SecurityConfig>().AuthenticationMode != AuthenticationMode.Claims)
            {
                SecurityManager.Logout();
            }
            else
            {
                IOwinContext owinContext = SystemManager.CurrentHttpContext.Request.GetOwinContext();
                string[] array = ClaimsManager.CurrentAuthenticationModule.GetSignOutAuthenticationTypes().ToArray();
                owinContext.Authentication.SignOut();
            }
        }
    }
}
