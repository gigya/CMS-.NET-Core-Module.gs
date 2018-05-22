using Gigya.Module.Core.Connector.Events;
using Sitecore.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sitecore.Gigya.Module.Pipelines
{
    public class RegisterPipelines
    {
        public void Process(PipelineArgs args)
        {
            GigyaEventHub.Instance.GetAccountInfoCompleted += Instance_GetAccountInfoCompleted;
            GigyaEventHub.Instance.AccountInfoMergeCompleted += Instance_AccountInfoMergeCompleted;
        }

        private void Instance_AccountInfoMergeCompleted(object sender, AccountInfoMergeCompletedEventArgs e)
        {
            var pipelineService = new PipelineService();
            pipelineService.RunGetAccountInfoMergeCompleted(e);
        }

        private void Instance_GetAccountInfoCompleted(object sender, GetAccountInfoCompletedEventArgs e)
        {
            var pipelineService = new PipelineService();
            pipelineService.RunGetAccountInfoCompleted(e);
        }
    }
}