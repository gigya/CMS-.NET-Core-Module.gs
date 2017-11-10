using Gigya.Module.Connector.Helpers;
using Gigya.Module.Connector.Logging;
using Gigya.Module.Core.Connector.Helpers;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.Core.Mvc.Controllers;
using Gigya.Module.Core.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Telerik.Sitefinity.Frontend.Mvc.Helpers;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Security.Claims;
using Telerik.Sitefinity.Services;

namespace Gigya.Module.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "GigyaSettings", Title = "Gigya Settings", SectionName = ModuleClass.WidgetSectionName)]
    public class GigyaSettingsController : BaseController
    {
        private Logger _logger;
        private Gigya.Module.Connector.Helpers.GigyaSettingsHelper _settingsHelper;

        public GigyaSettingsController()
        {
            _settingsHelper = new Gigya.Module.Connector.Helpers.GigyaSettingsHelper();
            _logger = new Logger(new SitefinityLogger());
        }

        public virtual ActionResult Index()
        {
            if (SystemManager.IsDesignMode || SystemManager.IsPreviewMode)
            {
                return new EmptyResult();
            }

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
            GigyaAccountHelper.ProcessRequestChecks(System.Web.HttpContext.Current, settings);

            var identity = ClaimsManager.GetCurrentIdentity();
            var currentIdentity = new CurrentIdentity
            {
                IsAuthenticated = identity.IsAuthenticated,
                Name = identity.Name
            };

            var viewModel = _settingsHelper.ViewModel(settings, Url, currentIdentity);
            viewModel.ErrorMessage = Res.Get(Constants.Resources.ClassId, Constants.Resources.ErrorMessage);
            Page pageHandler = HttpHandlerExtensions.GetPageHandler(HttpContext.CurrentHandler);

            // check if the widget is being rendered through Sitefinity or directly from the Razor view
            viewModel.RenderScript = pageHandler == null || pageHandler.Header == null || pageHandler.Header.Controls == null;

            if (!viewModel.RenderScript)
            {
                var script = this.GetScript(viewModel);
                pageHandler.Header.Controls.Add((Control)new LiteralControl(script));
            }
            
            var viewPath = FileHelper.GetPath("~/Mvc/Views/GigyaSettings/Index.cshtml", ModuleClass.ModuleVirtualPath + "Gigya.Module.Mvc.Views.GigyaSettings.Index.cshtml");
            return View(viewPath, viewModel);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            Index().ExecuteResult(this.ControllerContext);
        }

        private string GetScript(GigyaSettingsViewModel model)
        {
            var builder = new StringBuilder();
            builder.AppendFormat(@"<script>
                var gigyaConfig = {{
                    errorMessage: '{0}',
                    id: '{1}',
                    loggedInRedirectUrl: '{2}',
                    logoutRedirectUrl: '{3}',
                    debugMode: {4},
                    authenticated: {5}
                }};
            </script>", HttpUtility.JavaScriptStringEncode(model.ErrorMessage),
                        model.Id,
                        HttpUtility.JavaScriptStringEncode(model.LoggedInRedirectUrl),
                        HttpUtility.JavaScriptStringEncode(model.LogoutUrl),
                        model.DebugMode.ToString().ToLowerInvariant(),
                        model.IsLoggedIn.ToString().ToLowerInvariant()
            );
            builder.AppendFormat("<script src=\"https://cdns.{0}/js/gigya.js?apiKey={1}\">", model.DataCenter, model.ApiKey);
            builder.Append(model.SettingsJson);
            builder.Append("</script>");
            builder.AppendFormat("<script src=\"{0}\"></script>", model.GigyaScriptPath);
            if (model.DebugMode)
            {
                builder.Append("<script>window.gigyaDebugMode = true;</script>");
            }
            return builder.ToString();
        }
    }
}
