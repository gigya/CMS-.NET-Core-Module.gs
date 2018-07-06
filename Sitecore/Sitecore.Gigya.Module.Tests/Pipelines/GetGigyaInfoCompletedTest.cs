using Sitecore.Gigya.Module.Helpers;
using Sitecore.Gigya.Module.Pipelines;
using Sitecore.Gigya.Module.Repositories;
using Sitecore.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sitecore.Gigya.Module.Tests.Pipelines
{
    public class GetGigyaInfoCompletedTest
    {
        public void Process(GetAccountInfoCompletedPipelineArgs args)
        {
            // add a new field
            args.EventArgs.GigyaModel.profile.customField = "hello this is a custom value";

            // overwrite an existing field
            args.EventArgs.GigyaModel.profile.firstName = "GetGigyaInfoCompletedTest";
        }
    }
}
