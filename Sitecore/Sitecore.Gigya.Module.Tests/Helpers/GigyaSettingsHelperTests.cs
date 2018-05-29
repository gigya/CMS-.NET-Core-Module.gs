using FluentAssertions;
using Sitecore.Gigya.Module.Helpers;
using Sitecore.Gigya.Testing.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Xunit;

namespace Sitecore.Gigya.Module.Tests.Helpers
{
    public class GigyaSettingsHelperTests
    {
        [Theory]
        [AutoDbData]
        public void CanReadVersionFromXmlFile(FakeGigyaSettingsHelper gigyaSettingsHelper)
        {
            gigyaSettingsHelper.CmsMajorVersion.ShouldBeEquivalentTo("8");
            gigyaSettingsHelper.CmsVersion.ShouldBeEquivalentTo("8.2.180406");
            gigyaSettingsHelper.CmsName.ShouldBeEquivalentTo("Sitecore");
        }
    }
}
