using Gigya.Module.Core.Connector.Helpers;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.Core.Mvc.Models;
using Sitecore.Gigya.Extensions.Repositories;
using Sitecore.Gigya.Module.Helpers;
using Sitecore.Gigya.Module.Logging;
using Sitecore.Gigya.Module.Models;
using Sitecore.Gigya.Module.Repositories;
using Sitecore.Globalization;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Sitecore.Gigya.Module.Controllers
{
    public class GigyaController : Controller
    {
        private readonly IRenderingPropertiesRepository _renderingPropertiesRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly Logger _logger;
        private readonly IGigyaSettingsHelper _settingsHelper;

        public GigyaController() : this(new AccountRepository(new Pipelines.PipelineService()), new RenderingPropertiesRepository(), new Logger(new SitecoreLogger()), new Helpers.GigyaSettingsHelper())
        {
        }

        public GigyaController(IAccountRepository accountRepository, IRenderingPropertiesRepository renderingPropertiesRepository, Logger logger, IGigyaSettingsHelper settingsHelper)
        {
            _accountRepository = accountRepository;
            _renderingPropertiesRepository = renderingPropertiesRepository;
            _logger = logger;
            _settingsHelper = settingsHelper;
        }

        public ActionResult EditProfile()
        {
            var currentIdentity = _accountRepository.CurrentIdentity;
            if (!currentIdentity.IsAuthenticated)
            {
                return new EmptyResult();
            }

            var renderingModel = _renderingPropertiesRepository.Get<GigyaEditProfileRenderingModel>(RenderingContext.Current.Rendering);

            // check if Sitefinity is the session leader and sign in if required
            //GigyaAccountHelper.ProcessRequestChecks(System.Web.HttpContext.Current);

            if (renderingModel.RenderMethod != GigyaRenderingMethod.Embedded)
            {
                //renderingModel.GenerateContainer = false;
                renderingModel.ContainerId = null;
            }

            //var viewPath = FileHelper.GetPath("~/Mvc/Views/GigyaEditProfile/Index.cshtml", ModuleClass.ModuleVirtualPath + "Gigya.Module.Mvc.Views.GigyaEditProfile.Index.cshtml");
            var model = new GigyaEditProfileViewModel
            {
                Label = StringHelper.FirstNotNullOrEmpty(renderingModel.Label, "Edit Profile"),
                ContainerId = renderingModel.ContainerId,
                GeneratedContainerId = string.Concat("gigya-container-", Guid.NewGuid()),
                MobileScreenSet = renderingModel.MobileScreenSet,
                ScreenSet = StringHelper.FirstNotNullOrEmpty(renderingModel.ScreenSet, "Default-ProfileUpdate"),
                StartScreen = renderingModel.StartScreen
            };

            if (renderingModel.RenderMethod == GigyaRenderingMethod.Embedded && string.IsNullOrEmpty(model.ContainerId))
            {
                model.ContainerId = model.GeneratedContainerId;
            }

            return View(model);
        }

        public ActionResult Login()
        {
            var currentIdentity = _accountRepository.CurrentIdentity;
            if (currentIdentity.IsAuthenticated)
            {
                // check if Sitefinity is the session leader and sign in if required
                //GigyaAccountHelper.ProcessRequestChecks(System.Web.HttpContext.Current);
                return new EmptyResult();
            }

            var renderingModel = _renderingPropertiesRepository.Get<GigyaLoginRenderingModel>(RenderingContext.Current.Rendering);
            if (renderingModel.RenderMethod != GigyaRenderingMethod.Embedded)
            {
                //renderingModel.GenerateContainer = false;
                renderingModel.ContainerId = null;
            }

            //var viewPath = FileHelper.GetPath("~/Mvc/Views/GigyaLogin/Index.cshtml", ModuleClass.ModuleVirtualPath + "Gigya.Module.Mvc.Views.GigyaLogin.Index.cshtml");
            var model = new GigyaLoginViewModel
            {
                Label = StringHelper.FirstNotNullOrEmpty(renderingModel.Label, "Login"),
                ContainerId = renderingModel.ContainerId,
                GeneratedContainerId = string.Concat("gigya-container-", Guid.NewGuid()),
                MobileScreenSet = renderingModel.MobileScreenSet,
                ScreenSet = StringHelper.FirstNotNullOrEmpty(renderingModel.ScreenSet, "Default-RegistrationLogin"),
                StartScreen = renderingModel.StartScreen,
                LoggedInUrl = renderingModel.LoggedInUrl
            };

            if (renderingModel.RenderMethod == GigyaRenderingMethod.Embedded && string.IsNullOrEmpty(model.ContainerId))
            {
                model.ContainerId = model.GeneratedContainerId;
            }

            return View(model);
        }

        public ActionResult Logout()
        {
            var currentIdentity = _accountRepository.CurrentIdentity;
            if (!currentIdentity.IsAuthenticated)
            {
                return new EmptyResult();
            }

            // check if Sitefinity is the session leader and sign in if required
            //GigyaAccountHelper.ProcessRequestChecks(System.Web.HttpContext.Current);
            
            var renderingModel = _renderingPropertiesRepository.Get<GigyaLogoutRenderingModel>(RenderingContext.Current.Rendering);
            var model = new GigyaLogoutViewModel
            {
                Label = StringHelper.FirstNotNullOrEmpty(renderingModel.Label, "Logout"),
                LoggedOutUrl = renderingModel.LoggedOutUrl
            };

            return View(model);
        }

        public ActionResult Register()
        {
            var currentIdentity = _accountRepository.CurrentIdentity;
            if (currentIdentity.IsAuthenticated)
            {
                // check if Sitefinity is the session leader and sign in if required
                //GigyaAccountHelper.ProcessRequestChecks(System.Web.HttpContext.Current);
                return new EmptyResult();
            }

            var renderingModel = _renderingPropertiesRepository.Get<GigyaRegisterRenderingModel>(RenderingContext.Current.Rendering);
            if (renderingModel.RenderMethod != GigyaRenderingMethod.Embedded)
            {
                //renderingModel.GenerateContainer = false;
                renderingModel.ContainerId = null;
            }

            var model = new GigyaRegisterViewModel
            {
                Label = StringHelper.FirstNotNullOrEmpty(renderingModel.Label, "Register"),
                ContainerId = renderingModel.ContainerId,
                GeneratedContainerId = string.Concat("gigya-container-", Guid.NewGuid()),
                MobileScreenSet = renderingModel.MobileScreenSet,
                ScreenSet = StringHelper.FirstNotNullOrEmpty(renderingModel.ScreenSet, "Default-RegistrationLogin"),
                StartScreen = StringHelper.FirstNotNullOrEmpty(renderingModel.StartScreen, "gigya-register-screen"),
                LoggedInUrl = renderingModel.LoggedInUrl
            };

            if (renderingModel.RenderMethod == GigyaRenderingMethod.Embedded && string.IsNullOrEmpty(model.ContainerId))
            {
                model.ContainerId = model.GeneratedContainerId;
            }

            return View(model);
        }

        public virtual ActionResult Settings()
        {
            var settings = _settingsHelper.GetForCurrentSite(true);
            if (!settings.EnableRaas)
            {
                if (settings.DebugMode)
                {
                    _logger.Debug("RaaS disabled so GigyaSettings not added to page.");
                }
                return new EmptyResult();
            }

            if (string.IsNullOrEmpty(settings.ApiKey))
            {
                _logger.Error("Gigya API key not specified. Check settings on Administration -> Settings -> Gigya");
                return new EmptyResult();
            }

            // check if Sitefinity is the session leader and sign in if required
            //GigyaAccountHelper.ProcessRequestChecks(System.Web.HttpContext.Current, settings);

            var identity = _accountRepository.CurrentIdentity;
            var currentIdentity = new CurrentIdentity
            {
                IsAuthenticated = identity.IsAuthenticated,
                Name = identity.Name
            };

            var renderingModel = _renderingPropertiesRepository.Get<GigyaSettingsRenderingModel>(RenderingContext.Current.Rendering);
            var viewModel = Mapper.Map(_settingsHelper.ViewModel(settings, Url, currentIdentity));
            viewModel.ErrorMessage = Translate.TextByDomain(Constants.Dictionary.Domain, Constants.Dictionary.GenericError);
            viewModel.RenderScript = true;

            return View(viewModel);
        }
    }
}