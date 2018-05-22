using Gigya.Module.Core.Connector.Events;
using Sitecore.Pipelines;
using Sitecore.Security.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Gigya.Module.Pipelines
{
    public class PipelineService
    {
        public bool RunLoggedIn(User user)
        {
            var args = new AccountPipelineArgs()
            {
                User = user
            };
            CorePipeline.Run("gigya.module.loggedIn", args, false);
            return args.Aborted;
        }

        public bool RunLoggedOut(User user)
        {
            var args = new AccountPipelineArgs()
            {
                User = user
            };
            CorePipeline.Run("gigya.module.loggedOut", args, false);
            return args.Aborted;
        }

        public bool RunRegistered(User user)
        {
            var args = new AccountPipelineArgs()
            {
                User = user
            };
            CorePipeline.Run("gigya.module.registered", args, false);
            return args.Aborted;
        }

        public bool RunGetGigyaField(GigyaGetFieldEventArgs args)
        {
            CorePipeline.Run("gigya.module.getGigyaField", args, false);
            return args.Aborted;
        }

        public bool RunGetAccountInfoCompleted(GetAccountInfoCompletedEventArgs e)
        {
            var args = new GetAccountInfoCompletedPipelineArgs
            {
                CurrentSiteId = e.CurrentSiteId,
                GigyaModel = e.GigyaModel,
                Logger = e.Logger,
                MappingFields = e.MappingFields,
                Settings = e.Settings
            };

            CorePipeline.Run("gigya.module.getAccountInfoCompleted", args, false);
            return args.Aborted;
        }

        public bool RunGetAccountInfoMergeCompleted(AccountInfoMergeCompletedEventArgs e)
        {
            var args = new AccountInfoMergeCompletedPipelineArgs
            {
                CurrentSiteId = e.CurrentSiteId,
                GigyaModel = e.GigyaModel,
                Logger = e.Logger,
                Settings = e.Settings
            };

            CorePipeline.Run("gigya.module.getAccountInfoMergeCompleted", args, false);
            return args.Aborted;
        }
    }
}