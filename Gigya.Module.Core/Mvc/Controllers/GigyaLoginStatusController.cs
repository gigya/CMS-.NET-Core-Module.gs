using Gigya.Module.Core.Connector.Helpers;
using Gigya.Module.Core.Mvc.Models;
using Gigya.Module.Core.Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gigya.Module.Core.Mvc.Controllers
{
    public abstract class GigyaLoginStatusController : Controller
    {
        public Guid? LogoutPageId { get; set; }
        public Guid? RedirectPageId { get; set; }

        // GET: LoginStatus
        public ActionResult Index()
        {
            //var viewPath = FileHelper.GetPath("~/Mvc/Views/GigyaLoginStatus/Index.cshtml", ModuleClass.ModuleVirtualPath + "Gigya.Module.Core.Mvc.Views.GigyaLoginStatus.Index.cshtml");
            //var currentIdentity = ClaimsManager.GetCurrentIdentity();
            //GigyaLoginStatusViewModel model = GetViewModel(currentIdentity);

            //return View(viewPath, model);

            throw new NotImplementedException();
        }

        protected abstract GigyaLoginStatusViewModel GetViewModel(CurrentIdentity currentIdentity);
    }
}