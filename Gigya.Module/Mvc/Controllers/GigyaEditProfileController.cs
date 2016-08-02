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
    public class GigyaEditProfileController : Controller
    {
        public string RenderMethod { get; set; }
        public string ScreenSet { get; set; }
        public string MobileScreenSet { get; set; }
        public string StartScreen { get; set; }
        public string ContainerId { get; set; }
        public bool GenerateContainer { get; set; }
        public string Label { get; set; }

        // GET: LoginStatus
        public ActionResult Index()
        {
            var currentIdentity = ClaimsManager.GetCurrentIdentity();
            if (!currentIdentity.IsAuthenticated)
            {
                return new EmptyResult();
            }

            if (RenderMethod != Constants.Resources.Designer.EmbeddedScreen)
            {
                GenerateContainer = false;
                ContainerId = null;
            }

            var viewPath = FileHelper.GetPath("~/Mvc/Views/GigyaEditProfile/Index.cshtml", ModuleClass.ModuleVirtualPath + "Gigya.Module.Mvc.Views.GigyaEditProfile.Index.cshtml");
            var model = new GigyaEditProfileViewModel
            {
                Label = StringHelper.FirstNotNullOrEmpty(Label, "Edit Profile"),
                ContainerId = ContainerId,
                GeneratedContainerId = string.Concat("gigya-container-", Guid.NewGuid()),
                MobileScreenSet = StringHelper.FirstNotNullOrEmpty(MobileScreenSet, "DefaultMobile-ProfileUpdate"),
                ScreenSet = StringHelper.FirstNotNullOrEmpty(ScreenSet, "Default-ProfileUpdate"),
                StartScreen = StartScreen
            };

            if (RenderMethod == Constants.Resources.Designer.EmbeddedScreen && (GenerateContainer || string.IsNullOrEmpty(model.ContainerId)))
            {
                model.ContainerId = model.GeneratedContainerId;
            }

            return View(viewPath, model);
        }
    }
}