using System;
using System.Linq;
using System.Web.Mvc;
using Gigya.Module.Core.Connector.Helpers;
using Gigya.Module.Core.Mvc.Models;
using Gigya.Module.Core.Connector.Logging;

namespace Gigya.Module.Core.Mvc.Controllers
{
    public abstract class AccountController : BaseController
    {
        protected IGigyaMembershipHelper MembershipHelper { set; get; }
        protected GigyaSettingsHelper SettingsHelper { set; get; }
        protected Logger Logger { set; get; }

        public AccountController()
        {

        }

        public AccountController(IGigyaMembershipHelper membershipHelper, GigyaSettingsHelper settingsHelper, Logger logger)
        {
            MembershipHelper = membershipHelper;
            SettingsHelper = settingsHelper;
            Logger = logger;
        }

        protected abstract void Signout();
        protected abstract CurrentIdentity GetCurrentIdentity();

        [HttpPost]
        public virtual ActionResult EditProfile(LoginModel model)
        {
            var response = new LoginResponseModel();
            var id = model != null ? model.Id : null;
            var settings = SettingsHelper.Get(id, true);

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
            var currentIdentity = GetCurrentIdentity();

            if (currentIdentity.IsAuthenticated && currentIdentity.Name == model.UserId)
            {
                // already logged in
                response.Status = ResponseStatus.AlreadyLoggedIn;
                return JsonNetResult(response);
            }

            var id = model != null ? model.Id : null;
            var settings = SettingsHelper.Get(id, true);

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
        /// Should be called when a user is logged out of Gigya and therefore needs to be logged out of the CMS.
        /// </summary>
        public virtual ActionResult Logout(object id = null)
        {
            var settings = SettingsHelper.Get(id);
            
            var response = new ResponseModel { Status = ResponseStatus.Success };
            var currentIdentity = GetCurrentIdentity();

            if (!currentIdentity.IsAuthenticated)
            {
                // not logged in so just return success
                return JsonNetResult(response);
            }

            Signout();

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
            Signout();
            return new HttpStatusCodeResult(200);
        }
    }
}
