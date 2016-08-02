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
    public class GigyaLogoutController : Controller
    {
        public string Label { get; set; }
        public Guid? LogoutPageId { get; set; }

        // GET: LoginStatus
        public ActionResult Index()
        {
            var currentIdentity = ClaimsManager.GetCurrentIdentity();
            if (!currentIdentity.IsAuthenticated)
            {
                return new EmptyResult();
            }

            var viewPath = FileHelper.GetPath("~/Mvc/Views/GigyaLogout/Index.cshtml", ModuleClass.ModuleVirtualPath + "Gigya.Module.Mvc.Views.GigyaLogout.Index.cshtml");

            var model = new GigyaLogoutViewModel
            {
                Label = StringHelper.FirstNotNullOrEmpty(Label, "Logout"),
                LoggedOutUrl = SitefinityUtils.GetPageUrl(LogoutPageId)
            };

            return View(viewPath, model);
        }
    }
}