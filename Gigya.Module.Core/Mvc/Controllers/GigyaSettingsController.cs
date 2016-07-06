using Gigya.Module.Core.Connector.Common;
using Gigya.Module.Core.Connector.Helpers;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.Core.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;

namespace Gigya.Module.Core.Mvc.Controllers
{
    //[ControllerToolboxItem(Name = "GigyaSettings", Title = "Gigya Settings", SectionName = ModuleClass.WidgetSectionName)]
    //public class GigyaSettingsController : Controller
    //{
    //    public virtual ActionResult Index()
    //    {
    //        if (SystemManager.IsDesignMode)
    //        {
    //            return new EmptyResult();
    //        }

    //        var settings = GigyaSettingsHelper.GetForCurrentSite();
    //        if (!settings.EnableRaas)
    //        {
    //            if (settings.DebugMode)
    //            {
    //                Logger.Instance.Debug("RaaS disabled so GigyaSettings not added to page.");
    //            }
    //            return new EmptyResult();
    //        }

    //        if (string.IsNullOrEmpty(settings.ApiKey))
    //        {
    //            Logger.Instance.Error("Gigya API key not specified. Check settings on Administration -> Settings -> Gigya");
    //            return new EmptyResult();
    //        }

    //        var viewModel = GigyaSettingsHelper.ViewModel(settings, Url);
    //        Page pageHandler = HttpHandlerExtensions.GetPageHandler(HttpContext.CurrentHandler);

    //        // check if the widget is being rendered through Sitefinity or directly from the Razor view
    //        viewModel.RenderScript = pageHandler == null || pageHandler.Header == null || pageHandler.Header.Controls == null;

    //        if (!viewModel.RenderScript)
    //        {
    //            var script = this.GetScript(viewModel);
    //            pageHandler.Header.Controls.Add((Control)new LiteralControl(script));
    //        }
            
    //        var viewPath = FileHelper.GetPath("~/Mvc/Views/GigyaSettings/Index.cshtml", ModuleClass.ModuleVirtualPath + "Gigya.Module.Core.Mvc.Views.GigyaSettings.Index.cshtml");
    //        return View(viewPath, viewModel);
    //    }

    //    private string GetScript(GigyaSettingsViewModel model)
    //    {
    //        var builder = new StringBuilder();
    //        builder.AppendFormat("<script src=\"https://cdns.gigya.com/js/gigya.js?apiKey={0}\">", model.ApiKey);
    //        builder.Append(model.SettingsJson);
    //        builder.Append("</script>");
    //        builder.AppendFormat("<script src=\"{0}\"></script>", model.GigyaScriptPath);
    //        if (model.DebugMode)
    //        {
    //            builder.Append("<script>window.gigyaDebugMode = true;</script>");
    //        }
    //        return builder.ToString();
    //    }
    //}
}
