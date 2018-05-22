using Sitecore.Pipelines;
using Sitecore.Security.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Gigya.Module.Pipelines
{
    public class AccountPipelineArgs : PipelineArgs
    {
        public User User { get; set; }
    }
}