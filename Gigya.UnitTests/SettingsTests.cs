using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gigya.Module.Core.Connector.Helpers;

using Moq;
using Gigya.Umbraco.Module.Mvc.Models;
using System.Collections.Generic;
using Gigya.Module.Core.Data;
using Gigya.Module.Core.Mvc.Models;
using Gigya.Umbraco.Module.Data;
using Newtonsoft.Json;

namespace Gigya.UnitTests
{
    [TestClass]
    public class SettingsTests
    {
        [TestMethod]
        public void GlobalSettingsHasHigherPriority()
        {
            // arrange
            var currentIdentity = new CurrentIdentity();
            IGigyaModuleSettings settings = new GigyaModuleSettings
            {
                SessionTimeout = 10,
                Language = "abc",
                GlobalParameters = "{ \"sessionExpiration\": 123, \"lang\": \"override\", \"jkglobal\": \"hello\" }"
            };

            var pathUtility = new Mock<IPathUtilities>();
            pathUtility.Setup(i => i.ToAbsolute(It.IsAny<string>())).Returns(string.Empty);
            var settingsHelper = new Gigya.Umbraco.Module.Helpers.GigyaSettingsHelper();
            settingsHelper.PathUtilities = pathUtility.Object;

            var viewModel = settingsHelper.ViewModel(settings, null, currentIdentity);
            
            Assert.AreEqual(123, viewModel.Settings.sessionExpiration, "sessionExpiration doesn't match global params");

            Assert.AreEqual("override", viewModel.Settings.lang, "lang doesn't match global params");
            Assert.AreEqual("hello", viewModel.Settings.jkglobal, "global param doesn't exist or match in settings object");
        }
    }
}
