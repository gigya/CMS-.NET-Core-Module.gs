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
    public partial class EditProfile : System.Web.UI.UserControl
    {
        private readonly Helpers.GigyaSettingsHelper _settingsHelper = new Helpers.GigyaSettingsHelper();
        private readonly Logger _logger = new Logger(new SitecoreLogger());
        private readonly IAccountRepository _accountRepository = new AccountRepository(new Pipelines.PipelineService());
        private readonly IRenderingPropertiesRepository _renderingPropertiesRepository = new RenderingPropertiesRepository();

        private GigyaEditProfileViewModel Model()
        {
            var currentIdentity = _accountRepository.CurrentIdentity;
            if (!currentIdentity.IsAuthenticated)
            {
                return null;
            }

            var renderingModel = _renderingPropertiesRepository.Get<GigyaEditProfileRenderingModel>(Attributes["sc_parameters"]);

            if (renderingModel.RenderMethod != GigyaRenderingMethod.Embedded)
            {
                renderingModel.ContainerId = null;
            }

            var model = new GigyaEditProfileViewModel
            {
                Label = StringHelper.FirstNotNullOrEmpty(renderingModel.Label, "Edit Profile"),
                ContainerId = renderingModel.ContainerId,
                GeneratedContainerId = string.Concat("gigya-container-", Guid.NewGuid()),
                ScreenSet = StringHelper.FirstNotNullOrEmpty(renderingModel.ScreenSet, "Default-ProfileUpdate"),
                StartScreen = renderingModel.StartScreen
            };

            if (renderingModel.RenderMethod == GigyaRenderingMethod.Embedded && string.IsNullOrEmpty(model.ContainerId))
            {
                model.ContainerId = model.GeneratedContainerId;
            }

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

            if (!string.IsNullOrEmpty(model.ContainerId))
            {
                this.button.Visible = false;

                var pannel = new Panel();
                pannel.ClientIDMode = ClientIDMode.Static;
                pannel.ID = model.GeneratedContainerId;
                pannel.Attributes["data-gigya-container-id"] = model.ContainerId;
                pannel.CssClass = "gigya-cms-embedded-screen";
                pannel.Attributes["data-update-profile"] = "true";
                pannel.Attributes["data-gigya-screen"] = model.ScreenSet;
                pannel.Attributes["data-gigya-start-screen"] = model.StartScreen;

                EmbeddedContainer.Controls.Add(pannel);
            }
            else
            {
                this.EmbeddedContainer.Visible = false;

                button.Attributes["data-gigya-screen"] = model.ScreenSet;
                button.Attributes["data-gigya-start-screen"] = model.StartScreen;
                button.InnerText = model.Label;
            }
        }
    }
}