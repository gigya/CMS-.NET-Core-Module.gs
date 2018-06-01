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
using Gigya.Module.Core.Connector.Logging;
using Sitecore.Gigya.Testing.Logging;
using Gigya.Module.Core.Connector.Helpers;
using Sitecore.Gigya.Module.Models;

namespace Sitecore.Gigya.Module.Tests.Helpers
{
    public class GigyaAccountSchemaHelperTests
    {
        [Theory]
        [AutoDbData]
        public void CanGetAccountSchema(FakeGigyaSettingsHelper settingsHelper)
        {
            var logger = new Logger(new FakeCmsLogger());
            var settings = settingsHelper.Settings;
            var gigyaAccountSchemaHelper = new GigyaAccountSchemaHelper<SitecoreGigyaModuleSettings>(new GigyaApiHelper<SitecoreGigyaModuleSettings>(settingsHelper, logger), settings);

            var response = gigyaAccountSchemaHelper.GetAccountSchema();
            response.Should().NotBeNull();
            response.Properties.Should().NotBeNullOrEmpty();
        }
    }
}
