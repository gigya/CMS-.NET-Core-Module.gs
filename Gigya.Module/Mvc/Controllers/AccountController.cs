using System;
using System.Linq;
using System.Web.Mvc;
using Telerik.Sitefinity.Security;
using Gigya.Module.Connector.Logging;
using Gigya.Module.Connector.Helpers;
using Gigya.Module.Mvc.Models;
using Telerik.Sitefinity.Security.Claims;
using Telerik.Sitefinity.Abstractions;

namespace Gigya.Module.Mvc.Controllers
{
    public class AccountController : BaseController
    {
        private GigyaMembershipHelper _membershipHelper;

        protected virtual GigyaMembershipHelper MembershipHelper
        {
            get
            {
                if (_membershipHelper == null)
                {
                    _membershipHelper = ObjectFactory.Resolve<GigyaMembershipHelper>("GigyaMembershipHelper");
                }
                return _membershipHelper;
            }
        }

        [HttpPost]
        public virtual ActionResult EditProfile(LoginModel model)
        {
            var response = new LoginResponseModel();
            var siteId = model != null ? model.SiteId : Guid.Empty;
            var settings = GigyaSettingsHelper.Get(siteId, true);

            if (!ModelState.IsValid)
            {
                if (settings.DebugMode)
                {
                    var errorList = ModelState.Values.SelectMany(m => m.Errors)
                                     .Select(e => e.ErrorMessage)
                                     .ToList();

                    Logger.Debug("Invalid login request. ModelState Errors:\n" + string.Join(". ", errorList));
                }
                return JsonNetResult(response);
            }

            // attempt to update the user profile
            MembershipHelper.UpdateProfile(model, settings, ref response);
            return JsonNetResult(response);
        }

        [HttpPost]
        public virtual ActionResult Login(LoginModel model)
        {
            var response = new LoginResponseModel();
            var currentIdentity = ClaimsManager.GetCurrentIdentity();

            if (currentIdentity.IsAuthenticated && currentIdentity.Name == model.UserId)
            {
                // already logged in
                response.Status = ResponseStatus.AlreadyLoggedIn;
                return JsonNetResult(response);
            }

            var siteId = model != null ? model.SiteId : Guid.Empty;
            var settings = GigyaSettingsHelper.Get(siteId, true);

            if (!ModelState.IsValid)
            {
                if (settings.DebugMode)
                {
                    var errorList = ModelState.Values.SelectMany(m => m.Errors)
                                     .Select(e => e.ErrorMessage)
                                     .ToList();

                    Logger.Debug("Invalid login request. ModelState Errors:\n" + string.Join(". ", errorList));
                }
                return JsonNetResult(response);
            }

            // attempt to login or register the user
            MembershipHelper.LoginOrRegister(model, settings, ref response);                           
            return JsonNetResult(response);
        }

        /// <summary>
        /// Should be called when a user is logged out of Gigya and therefore needs to be logged out of Sitefinity.
        /// </summary>
        public virtual ActionResult Logout(Guid? siteId)
        {
            var settings = GigyaSettingsHelper.Get(siteId ?? Guid.Empty);
            
            var response = new ResponseModel { Status = ResponseStatus.Success };
            var currentIdentity = ClaimsManager.GetCurrentIdentity();

            if (!currentIdentity.IsAuthenticated)
            {
                // not logged in so just return success
                return JsonNetResult(response);
            }

            SecurityManager.Logout();

            if (settings.DebugMode)
            {
                Logger.Debug(currentIdentity.Name + " successfully logged out.");
            }

            response.RedirectUrl = settings.LogoutUrl;
            return JsonNetResult(response);
        }

        /// <summary>
        /// SSO Logout url which Gigya forwards the user to.
        /// </summary>
        public virtual ActionResult LogoutSSO()
        {
            SecurityManager.Logout();
            return new HttpStatusCodeResult(200);
        }
    }
}
