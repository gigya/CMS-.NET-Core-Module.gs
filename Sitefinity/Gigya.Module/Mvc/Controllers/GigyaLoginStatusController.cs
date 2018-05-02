using Gigya.Module.Connector.Helpers;
using Gigya.Module.Core.Connector.Helpers;
using Gigya.Module.Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Claims;
using Telerik.Sitefinity.Security.Model;
using Telerik.Sitefinity.Services;

namespace Gigya.Module.Mvc.Controllers
{
    public class GigyaLoginStatusController : Controller
    {
        public Guid? LogoutPageId { get; set; }
        public Guid? RedirectPageId { get; set; }

        // GET: LoginStatus
        public ActionResult Index()
        {
            var viewPath = FileHelper.GetPath("~/Mvc/Views/GigyaLoginStatus/Index.cshtml", ModuleClass.ModuleVirtualPath + "Gigya.Module.Mvc.Views.GigyaLoginStatus.Index.cshtml");
            var currentIdentity = ClaimsManager.GetCurrentIdentity();
            GigyaLoginStatusViewModel model = GetViewModel(currentIdentity);

            return View(viewPath, model);
        }

        protected virtual GigyaLoginStatusViewModel GetViewModel(ClaimsIdentityProxy currentIdentity)
        {
            var model = new GigyaLoginStatusViewModel
            {
                SiteId = SystemManager.CurrentContext.IsMultisiteMode ? SystemManager.CurrentContext.CurrentSite.Id : Guid.Empty,
                IsLoggedIn = currentIdentity.IsAuthenticated,
                IsDesignMode = SystemManager.IsDesignMode
            };

            if (model.IsLoggedIn)
            {
                // check if Sitefinity is the session leader and sign in if required
                GigyaAccountHelper.ProcessRequestChecks(System.Web.HttpContext.Current);

                var userManager = UserManager.GetManager();
                var currentUser = userManager.GetUser(currentIdentity.UserId);
                if (currentUser != null)
                {
                    model.Email = currentUser.Email;

                    var profileManager = UserProfileManager.GetManager();
                    var profile = profileManager.GetUserProfile<SitefinityProfile>(currentUser);
                    if (profile != null)
                    {
                        model.FirstName = profile.FirstName;
                        model.LastName = profile.LastName;
                    }
                }
                else
                {
                    model.IsLoggedIn = false;
                }
            }

            return model;
        }

        protected override void HandleUnknownAction(string actionName)
        {
            Index().ExecuteResult(this.ControllerContext);
        }
    }
}