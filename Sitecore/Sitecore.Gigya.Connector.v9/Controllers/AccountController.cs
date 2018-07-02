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
using Sitecore.Configuration;
using Sitecore.Gigya.Extensions.Abstractions;
using Sitecore.Gigya.Connector.Services;
using Sitecore.Gigya.Connector.Providers;

namespace Sitecore.Gigya.Connector.Controllers
{
    public class AccountController : Sitecore.Gigya.Module.Controllers.AccountController
    {
        public AccountController() : base(new AccountRepository(new Module.Pipelines.PipelineService()), new TrackerService(), 
            new ContactProfileService(new ContactProfileProvider(), new LegacyContactProfileProvider(), new Logger(new SitecoreLogger())),
            new Module.Helpers.GigyaSettingsHelper())
        {
        }
    }
}