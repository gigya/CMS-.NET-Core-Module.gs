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
    public class GetGigyaValueTest
    {
        public void Process(GigyaGetFieldEventArgs args)
        {
            // check that we can change the gigya value on the fly
            if (args.GigyaValue is string)
            {
                args.GigyaValue = new Random().Next(int.MaxValue).ToString();
            }
        }
    }
}
