using Gigya.Module.Core.Connector.Helpers;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.Core.Mvc.Models;
using Sitecore.Gigya.Module.Helpers;
using Sitecore.Gigya.Module.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SC = Sitecore;

using Core = Gigya.Module.Core;
using Sitecore.Gigya.Module.Repositories;
using Sitecore.Gigya.Module.Models;

namespace Sitecore.Gigya.Module.Controllers
{
    public class AccountController : Core.Mvc.Controllers.AccountController<SitecoreGigyaModuleSettings>
    {
        protected readonly IAccountRepository _accountRepository;
        protected readonly GigyaAccountHelper _gigyaAccountHelper;

        public AccountController() : this(new AccountRepository(new Pipelines.PipelineService()))
        {
        }

        public AccountController(IAccountRepository accountRepository) : base()
        {
            _accountRepository = accountRepository;
            Logger = new Logger(new SitecoreLogger());
            SettingsHelper = new Helpers.GigyaSettingsHelper();

            _gigyaAccountHelper = new GigyaAccountHelper(SettingsHelper, Logger, null, _accountRepository);
            var apiHelper = new GigyaApiHelper<SitecoreGigyaModuleSettings>(SettingsHelper, Logger);
            MembershipHelper = new GigyaMembershipHelper(apiHelper, Logger, _gigyaAccountHelper, _accountRepository);
        }

        protected override CurrentIdentity GetCurrentIdentity()
        {
            return _accountRepository.CurrentIdentity;
        }

        protected override void Signout()
        {
            _accountRepository.Logout();
        }
    }
}