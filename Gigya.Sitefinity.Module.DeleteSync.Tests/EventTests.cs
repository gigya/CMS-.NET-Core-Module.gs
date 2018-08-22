using System;
using System.Collections.Generic;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.DeleteSync.Events;
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
    public class EventTests
    {
        [TestMethod]
        public void TestDeletingEventIsRaised()
        {
            var provider = new FakeEmailProvider();
            var emailHelper = new EmailHelper(provider);
            var fakeLogger = new FakeCmsLogger();
            var logger = new Logger(fakeLogger);

            var userManager = new FakeUserManager();
            var profileManager = new FakeProfileManager();
            var context = new FakeGigyaDeleteSyncContext();

            var helper = new DeleteSyncHelper(emailHelper, logger, userManager, profileManager, context);

            DeleteSyncEventHub.Instance.DeletingUser += (sender, args) =>
            {
                args.ContinueWithDeletion = false;
            };

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
            Assert.IsTrue(logs.Contains("Deletion not required for user with UID of"));
        }
    }
}
