using Gigya.Umbraco.Module.Helpers;
using Gigya.Module.Core.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Umbraco.Module.Connector;
using Gigya.Module.Core.Mvc.Models;
using Gigya.Module.Core.Connector.Enums;
using Gigya.Module.Core.Data;
using Gigya.Module.Core.Connector.Helpers;
using Newtonsoft.Json;
using System.Web;
using Umbraco.Core;
using Gigya.Umbraco.Module.Connector.Helpers;

namespace Gigya.Umbraco.Module.Mvc.Controllers
{
    public class GigyaSettingsController : BaseController
    {
        private Logger _logger;
        private Helpers.GigyaSettingsHelper _settingsHelper;

        public GigyaSettingsController()
        {
            _settingsHelper = new Helpers.GigyaSettingsHelper();
            _logger = new Logger(new UmbracoLogger());
        }

        public virtual ActionResult Index()
        {
            var decryptApplicationSecret = false;
            var settings = _settingsHelper.GetForCurrentSite(decryptApplicationSecret);
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
                _logger.Error("Gigya API key not specified. Umbraco -> Gigya");
                return new EmptyResult();
            }

            var accountHelper = new GigyaAccountHelper(_settingsHelper, _logger);
            accountHelper.LoginToGigyaIfRequired();

            var currentIdentity = new CurrentIdentity
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                Name = User.Identity.Name
            };          

            var viewModel = _settingsHelper.ViewModel(settings, Url, currentIdentity);

            // umbraco doesn't use web forms so the script will always be rendered inline
            viewModel.RenderScript = true;

            var viewPath = "~/Views/GigyaSettings/Index.cshtml";
            return View(viewPath, viewModel);
        }
    }
}
