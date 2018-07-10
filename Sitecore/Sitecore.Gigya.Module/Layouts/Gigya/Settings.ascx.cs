using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.Core.Mvc.Models;
using Sitecore.Gigya.Extensions.Abstractions.Repositories;
using Sitecore.Gigya.Module.Controllers;
using Sitecore.Gigya.Module.Helpers;
using Sitecore.Gigya.Module.Logging;
using Sitecore.Gigya.Module.Models;
using Sitecore.Gigya.Module.Repositories;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sitecore.Gigya.Module.Layouts.Gigya
{
    public partial class Settings : System.Web.UI.UserControl
    {
        private readonly Helpers.GigyaSettingsHelper _settingsHelper = new Helpers.GigyaSettingsHelper();
        private readonly Logger _logger = new Logger(new SitecoreLogger());
        private readonly IAccountRepository _accountRepository = new AccountRepository(new Pipelines.PipelineService());
        private readonly IRenderingPropertiesRepository _renderingPropertiesRepository = new RenderingPropertiesRepository();

        private Models.GigyaSettingsViewModel Model()
        {
            var settings = _settingsHelper.GetForCurrentSite(true);
            if (!settings.EnableRaas)
            {
                if (settings.DebugMode)
                {
                    _logger.Debug("RaaS disabled so GigyaSettings not added to page.");
                }
                return null;
            }

            if (string.IsNullOrEmpty(settings.ApiKey))
            {
                _logger.Error("Gigya API key not specified. Check settings on Administration -> Settings -> Gigya");
                return null;
            }

            var identity = _accountRepository.CurrentIdentity;
            var currentIdentity = new CurrentIdentity
            {
                IsAuthenticated = identity.IsAuthenticated,
                Name = identity.Name
            };

            var renderingModel = _renderingPropertiesRepository.Get<GigyaSettingsRenderingModel>(Attributes["sc_parameters"]);
            var viewModel = Mapper.Map(_settingsHelper.ViewModel(settings, null, currentIdentity));
            viewModel.ErrorMessage = Translate.TextByDomain(Constants.Dictionary.Domain, Constants.Dictionary.GenericError);
            viewModel.RenderScript = true;
            return viewModel;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            var model = Model();

            var builder = new StringBuilder();

            builder.AppendFormat(
                @"<script>
                    var gigyaConfig = {{
                        errorMessage: '{0}',
                        id: '{1}',
                        loggedInRedirectUrl: '{2}',
                        logoutRedirectUrl: '{3}',
                        debugMode: {4},
                        authenticated: {5},
                        getInfoRequired: {6},
                        enableSSOToken: {7}
                    }};
                </script>", 
                HttpUtility.JavaScriptStringEncode(model.ErrorMessage), HttpUtility.HtmlEncode(model.Id), HttpUtility.JavaScriptStringEncode(model.LoggedInRedirectUrl),
                HttpUtility.JavaScriptStringEncode(model.LogoutUrl), model.DebugMode.ToString().ToLowerInvariant(), model.IsLoggedIn.ToString().ToLowerInvariant(),
                model.IsGetInfoRequired.ToString().ToLowerInvariant(), model.EnableSSOToken.ToString().ToLowerInvariant());

            builder.AppendFormat(@"<script src=""//cdns.{0}/js/gigya.js?apiKey={1}"">{2}</script>", model.DataCenter, model.ApiKey, model.SettingsJson);
            builder.AppendFormat(@"<script src=""{0}""></script>", model.GigyaScriptPath);

            if (model.DebugMode)
            {
                builder.Append("<script>window.gigyaDebugMode = true;</script>");
            }

            writer.Write(builder.ToString());
        }
    }
}