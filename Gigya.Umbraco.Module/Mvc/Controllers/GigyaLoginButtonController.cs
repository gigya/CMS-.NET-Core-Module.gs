//using Gigya.Umbraco.Module.Helpers;
//using Gigya.Module.Core.Mvc.Controllers;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web.Mvc;
//using Gigya.Module.Core.Connector.Logging;
//using Gigya.Umbraco.Module.Connector;

//namespace Gigya.Umbraco.Module.Mvc.Controllers
//{
//    public class GigyaSettingsController : BaseController
//    {
//        private Logger _logger;
//        private GigyaSettingsHelper _settingsHelper;

//        public GigyaSettingsController()
//        {
//            _settingsHelper = new GigyaSettingsHelper();
//            _logger = new Logger(new UmbracoLogger());
//        }

//        public virtual ActionResult Index()
//        {
//            var settings = _settingsHelper.GetForCurrentSite();
//            if (!settings.EnableRaas)
//            {
//                if (settings.DebugMode)
//                {
//                    _logger.Debug("RaaS disabled so GigyaSettings not added to page.");
//                }
//                return new EmptyResult();
//            }

//            if (string.IsNullOrEmpty(settings.ApiKey))
//            {
//                _logger.Error("Gigya API key not specified. Check settings on Administration -> Settings -> Gigya");
//                return new EmptyResult();
//            }

//            var viewModel = _settingsHelper.ViewModel(settings, Url);

//            // umbraco doesn't use web forms so the script will always be rendered inline
//            viewModel.RenderScript = true;

//            var viewPath = "~/Views/GigyaSettings/Index.cshtml";
//            return View(viewPath, viewModel);
//        }
//    }
//}
