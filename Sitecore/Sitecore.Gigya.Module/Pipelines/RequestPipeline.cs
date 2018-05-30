using Gigya.Module.Core.Connector.Events;
using Sitecore.Gigya.Module.Helpers;
using Sitecore.Gigya.Module.Logging;
using Sitecore.Gigya.Module.Repositories;
using Sitecore.Pipelines;
using Sitecore.Pipelines.HttpRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sitecore.Gigya.Module.Pipelines
{
    public class RequestPipeline
    {
        public void Process(HttpRequestArgs args)
        {
            var accountRepository = new AccountRepository(new PipelineService());
            var currentUser = accountRepository.GetActiveUser();
            if (currentUser.GetDomainName() == "sitecore")
            {
                return;
            }
            
            if (!IsPageLoadOrAjax(args))
            {
                return;
            }

            var settingsHelper = new GigyaSettingsHelper();
            var logger = LoggerFactory.Instance();
            var accountHelper = new GigyaAccountHelper(settingsHelper, logger, null, accountRepository);
            accountHelper.UpdateSessionExpirationCookieIfRequired(args.Context, false);
        }

        private bool IsPageLoadOrAjax(HttpRequestArgs args)
        {
            return Context.Item != null || new HttpRequestWrapper(args.Context.Request).IsAjaxRequest();
        }
    }
}