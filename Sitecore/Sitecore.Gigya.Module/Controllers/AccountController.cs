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
using Sitecore.Gigya.Extensions.Abstractions.Services;
using Sitecore.DependencyInjection;
using Sitecore.Configuration;
using Sitecore.Gigya.Extensions.Services;
using Sitecore.Gigya.Extensions.Providers;

namespace Sitecore.Gigya.Module.Controllers
{
    public class AccountController : Core.Mvc.Controllers.AccountController<SitecoreGigyaModuleSettings>
    {
        protected readonly IAccountRepository _accountRepository;
        protected readonly GigyaAccountHelper _gigyaAccountHelper;

        public AccountController() : this(new AccountRepository(new Pipelines.PipelineService()), new TrackerService(), new ContactProfileService(new ContactProfileProvider(), new Logger(new SitecoreLogger())))
        {
        }

        public AccountController(IAccountRepository accountRepository, ITrackerService trackerService, IContactProfileService contactProfileService) : base()
        {
            _accountRepository = accountRepository;
            Logger = new Logger(new SitecoreLogger());
            SettingsHelper = new Helpers.GigyaSettingsHelper();

            _gigyaAccountHelper = new GigyaAccountHelper(SettingsHelper, Logger, null, _accountRepository);
            var apiHelper = new GigyaApiHelper<SitecoreGigyaModuleSettings>(SettingsHelper, Logger);
            MembershipHelper = new GigyaMembershipHelper(apiHelper, Logger, _gigyaAccountHelper, _accountRepository, trackerService, contactProfileService);
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