﻿using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.Core.Connector.Models;
using Gigya.Module.Core.Data;
using Sitecore.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Gigya.Module.Pipelines
{
    public abstract class GetAccountInfoPipelineArgs : PipelineArgs
    {
        public Logger Logger { get; set; }
        public IGigyaModuleSettings Settings { get; set; }
        public dynamic GigyaModel { get; set; }
        public object CurrentSiteId { get; set; }
    }
}