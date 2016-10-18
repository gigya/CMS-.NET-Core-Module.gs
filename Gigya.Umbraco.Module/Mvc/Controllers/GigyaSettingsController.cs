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

            var currentIdentity = new CurrentIdentity
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                Name = User.Identity.Name
            };

            if (settings.SessionProvider == GigyaSessionProvider.CMS)
            {
                var cookieName = "glt_" + settings.ApiKey;
                if (currentIdentity.IsAuthenticated)
                {
                    if (Request.Cookies[cookieName] == null)
                    {
                        // user logged into Umbraco but not Gigya so call notifyLogin to sign in
                        if (!decryptApplicationSecret)
                        {
                            _settingsHelper.DecryptApplicationSecret(ref settings);
                        }
                        SignInUserAfterGigyaTimeout(settings, currentIdentity);
                    }
                }
            }            

            var viewModel = _settingsHelper.ViewModel(settings, Url, currentIdentity);

            // umbraco doesn't use web forms so the script will always be rendered inline
            viewModel.RenderScript = true;

            var viewPath = "~/Views/GigyaSettings/Index.cshtml";
            return View(viewPath, viewModel);
        }
        
        private void SignInUserAfterGigyaTimeout(IGigyaModuleSettings settings, CurrentIdentity currentIdentity)
        {
            var uid = currentIdentity.Name;
            var uidMapping = settings.MappedMappingFields.FirstOrDefault(i => i.GigyaFieldName == Constants.GigyaFields.UserId && !string.IsNullOrEmpty(i.CmsFieldName));
            if (uidMapping != null && uidMapping.CmsFieldName != Constants.CmsFields.Username)
            {
                // get member to find UID field
                var member = ApplicationContext.Current.Services.MemberService.GetByUsername(currentIdentity.Name);
                if (member == null)
                {
                    _logger.Error(string.Format("Couldn't find member with username of {0} so couldn't sign them in.", currentIdentity.Name));
                    return;
                }

                uid = member.GetValue<string>(uidMapping.CmsFieldName);
            }

            var helper = new GigyaApiHelper(_settingsHelper, _logger);
            var sessionExpiration = _settingsHelper.SessionExpiration(settings);
            var apiResponse = helper.NotifyLogin(uid, sessionExpiration, settings);

            if (apiResponse == null)
            {
                _logger.Error("Failed to call gigya.accounts.notifyLogin for user " + currentIdentity.Name);
                return;
            }

            var notifyLoginResponse = JsonConvert.DeserializeObject<NotifyLoginResponse>(apiResponse.GetResponseText());
            if (notifyLoginResponse.sessionInfo != null)
            {
                var cookie = new HttpCookie(notifyLoginResponse.sessionInfo.cookieName, notifyLoginResponse.sessionInfo.cookieValue);
                if (sessionExpiration > 0)
                {
                    cookie.Expires = DateTime.UtcNow.AddSeconds(sessionExpiration);
                }

                Response.Cookies.Add(cookie);
                _logger.DebugFormat("Successfully logged in user {0} using gigya.accounts.notifyLogin", currentIdentity.Name);
            }
            else
            {
                _logger.Error("No sessionInfo returned from gigya.accounts.notifyLogin for user " + currentIdentity.Name);
            }
        }
    }
}
