using Sitecore.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Gigya.Module.Pipelines
{
    public class GigyaGetFieldEventArgs : PipelineArgs
    {
        public dynamic GigyaModel { get; set; }
        public string CmsFieldName { get; set; }
        public string GigyaFieldName { get; set; }
        public object GigyaValue { get; set; }
        public string Origin { get; set; }
    }
}
