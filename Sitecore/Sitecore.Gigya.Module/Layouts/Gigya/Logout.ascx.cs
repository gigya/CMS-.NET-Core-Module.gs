using Gigya.Module.Core.Connector.Helpers;
using Gigya.Module.Core.Connector.Logging;
using Sitecore.Gigya.Extensions.Abstractions.Repositories;
using Sitecore.Gigya.Module.Fields;
using Sitecore.Gigya.Module.Logging;
using Sitecore.Gigya.Module.Models;
using Sitecore.Gigya.Module.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sitecore.Gigya.Module.Layouts.Gigya
{
    public partial class Logout : System.Web.UI.UserControl
    {
        private readonly Helpers.GigyaSettingsHelper _settingsHelper = new Helpers.GigyaSettingsHelper();
        private readonly Logger _logger = new Logger(new SitecoreLogger());
        private readonly IAccountRepository _accountRepository = new AccountRepository(new Pipelines.PipelineService());
        private readonly IRenderingPropertiesRepository _renderingPropertiesRepository = new RenderingPropertiesRepository();

        private Models.GigyaLogoutViewModel Model()
        {
            var currentIdentity = _accountRepository.CurrentIdentity;
            if (!currentIdentity.IsAuthenticated)
            {
                return null;
            }

            var renderingModel = _renderingPropertiesRepository.Get<GigyaLogoutRenderingModel>(Attributes["sc_parameters"]);
            var model = new GigyaLogoutViewModel
            {
                Label = StringHelper.FirstNotNullOrEmpty(renderingModel.Label, "Logout"),
                LoggedOutUrl = new ExtendedLinkUrl().GetUrl(renderingModel.LoggedOutUrl, Sitecore.Context.Database)
            };

            return model;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var model = Model();
            if (model == null)
            {
                this.Visible = false;
                return;
            }

            button.InnerText = model.Label;
            loggouturl.Value = model.LoggedOutUrl;
        }
    }
}