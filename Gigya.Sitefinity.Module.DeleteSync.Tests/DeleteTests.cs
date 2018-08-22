using System;
using System.Collections.Generic;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.DeleteSync.Helpers;
using Gigya.Module.DeleteSync.Models;
using Gigya.Module.DeleteSync.Tests.Logging;
using Gigya.Module.DeleteSync.Tests.Providers;
using Gigya.Sitefinity.Module.DeleteSync.Helpers;
using Gigya.Sitefinity.Module.DeleteSync.Models;
using Gigya.Sitefinity.Module.DeleteSync.Tests.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gigya.Sitefinity.Module.DeleteSync.Tests
{
    [TestClass]
    public class DeleteTests
    {
        [TestMethod]
        public void CanDeleteUser()
        {
            var provider = new FakeEmailProvider();
            var emailHelper = new EmailHelper(provider);
            var fakeLogger = new FakeCmsLogger();
            var logger = new Logger(fakeLogger);

            var userManager = new FakeUserManager();
            var profileManager = new FakeProfileManager();
            var context = new FakeGigyaDeleteSyncContext();

            var helper = new DeleteSyncHelper(emailHelper, logger, userManager, profileManager, context);

            var settings = new SitefinityDeleteSyncSettings
            {
                Enabled = true,
                MaxAttempts = 1,
                Action = DeleteSyncAction.FullUserDeletion,
                FrequencyMins = 60,
                EmailsOnSuccess = "success@test.com",
                EmailsOnFailure = "fail@test.com"
            };

            var files = new List<DeleteSyncFile>
            {
                new DeleteSyncFile
                {
                    Key = "1",
                    UIDs = new List<string>
                    {
                        "valid"
                    }
                }
            };

            helper.Process(settings, files);

            var logs = fakeLogger.Logs.ToString();
            Assert.IsTrue(logs.Contains("A total of"));

            Assert.IsTrue(logs.Contains($"User with UID of  has been permanently deleted."));
        }
    }
}
