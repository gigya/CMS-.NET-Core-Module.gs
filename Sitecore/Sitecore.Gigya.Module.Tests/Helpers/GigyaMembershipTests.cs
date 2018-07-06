using Sitecore.Gigya.Module.Helpers;
using Sitecore.Gigya.Testing.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sitecore.Gigya.Module.Tests.Helpers
{
    public class GigyaMembershipTests
    {
        [Theory]
        [AutoDbData]
        public void CanReadVersionFromXmlFile(GigyaMembershipHelper gigyaMembershipHelper)
        {
            //gigyaMembershipHelper.UpdateProfile
        }
    }
}
