using Sitecore.Analytics.Model.Framework;
using Sitecore.Gigya.Extensions.Abstractions.Analytics.Models;
using Sitecore.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Gigya.Module.Models.Pipelines
{
    public class FacetsUpdatedPipelineArgs : PipelineArgs
    {
        public MappingFieldGroup Mappings { get; set; }
        public dynamic GigyaModel { get; set; }
    }
}