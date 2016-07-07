using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gigya.Module.Core.Mvc.Models;
using System.Web.Security;
using Gigya.Umbraco.Module.Connector.Helpers;
using Gigya.Module.Core.Connector.Helpers;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Umbraco.Module.Connector;
using System.Web.Mvc;

namespace Gigya.Umbraco.Module.Mvc.Controllers
{
    public class AccountController : Gigya.Module.Core.Mvc.Controllers.AccountController
    {
        public AccountController() : base()
        {
            Logger = new Logger(new UmbracoLogger());
            SettingsHelper = new Helpers.GigyaSettingsHelper();
            var apiHelper = new GigyaApiHelper(SettingsHelper, Logger);
            MembershipHelper = new GigyaMembershipHelper(apiHelper, Logger);
        }

        protected override CurrentIdentity GetCurrentIdentity()
        {
            return new CurrentIdentity
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                Name = User.Identity.Name
            };
        }

        protected override void Signout()
        {
            FormsAuthentication.SignOut();
        }
    }
}
