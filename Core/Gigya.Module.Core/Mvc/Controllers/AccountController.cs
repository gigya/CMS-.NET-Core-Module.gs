using System;
using System.Linq;
using System.Web.Mvc;
using Gigya.Module.Core.Connector.Helpers;
using Gigya.Module.Core.Mvc.Models;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.Core.Data;
using Gigya.Module.Core.Connector.Enums;

namespace Gigya.Module.Core.Mvc.Controllers
{
    public abstract class AccountController : AccountController<GigyaModuleSettings>
    {
        public AccountController() : base()
        {

        }

        public AccountController(IGigyaMembershipHelper<GigyaModuleSettings> membershipHelper, IGigyaSettingsHelper<GigyaModuleSettings> settingsHelper, Logger logger) 
            : base(membershipHelper, settingsHelper, logger)
        {
        }
    }

    public abstract class AccountController<T> : BaseController where T: GigyaModuleSettings
    {
        protected IGigyaMembershipHelper<T> MembershipHelper { set; get; }
        protected IGigyaSettingsHelper<T> SettingsHelper { set; get; }
        protected Logger Logger { set; get; }

        public AccountController()
        {

        }

        public AccountController(IGigyaMembershipHelper<T> membershipHelper, IGigyaSettingsHelper<T> settingsHelper, Logger logger)
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
            var currentIdentity = GetCurrentIdentity();
            if (!currentIdentity.IsAuthenticated)
            {
                // not logged in
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

            // attempt to update the user profile
            MembershipHelper.UpdateProfile(model, settings, ref response);
            return JsonNetResult(response);
        }

        [HttpPost]
        public virtual ActionResult Login(LoginModel model)
        {
            var response = new LoginResponseModel();
            var currentIdentity = GetCurrentIdentity();

            if (currentIdentity.IsAuthenticated && model.UserId == currentIdentity.UID)
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

            LoginOrRegister(model, settings, ref response);
            return JsonNetResult(response);
        }

        /// <summary>
        /// Logsin or registers a user.
        /// </summary>
        protected virtual void LoginOrRegister(LoginModel model, T settings, ref LoginResponseModel response)
        {
            if (model.LoginSource == LoginSource.GetAccountInfo && settings.SessionProvider == GigyaSessionProvider.CMS)
            {
                // don't sign in user but sign them out if a client side login event hasn't been fired yet (SS0).
                response.Status = ResponseStatus.LogoutIfNoLoginFired;
                return;
            }

            // attempt to login or register the user
            MembershipHelper.LoginOrRegister(model, settings, ref response);
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
