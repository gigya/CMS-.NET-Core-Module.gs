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
    public class GigyaLoginController : Controller
    {
        public Guid? LoggedInPage { get; set; }
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
            if (currentIdentity.IsAuthenticated)
            {
                // check if Sitefinity is the session leader and sign in if required
                GigyaAccountHelper.ValidateAndLoginToGigyaIfRequired(System.Web.HttpContext.Current);
                return new EmptyResult();
            }

            if (RenderMethod != Constants.Resources.Designer.EmbeddedScreen)
            {
                GenerateContainer = false;
                ContainerId = null;
            }

            var viewPath = FileHelper.GetPath("~/Mvc/Views/GigyaLogin/Index.cshtml", ModuleClass.ModuleVirtualPath + "Gigya.Module.Mvc.Views.GigyaLogin.Index.cshtml");
            var model = new GigyaLoginViewModel
            {
                Label = StringHelper.FirstNotNullOrEmpty(Label, "Login"),
                ContainerId = ContainerId,
                GeneratedContainerId = string.Concat("gigya-container-", Guid.NewGuid()),
                MobileScreenSet = MobileScreenSet,
                ScreenSet = StringHelper.FirstNotNullOrEmpty(ScreenSet, "Default-RegistrationLogin"),
                StartScreen = StartScreen,
                LoggedInUrl = SitefinityUtils.GetPageUrl(LoggedInPage)
            };

            if (RenderMethod == Constants.Resources.Designer.EmbeddedScreen && (GenerateContainer || string.IsNullOrEmpty(model.ContainerId)))
            {
                model.ContainerId = model.GeneratedContainerId;
            }

            return View(viewPath, model);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            Index().ExecuteResult(this.ControllerContext);
        }
    }
}