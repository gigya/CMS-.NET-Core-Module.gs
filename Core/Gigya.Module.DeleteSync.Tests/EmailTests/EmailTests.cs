using System;
using System.Collections.Generic;
using Gigya.Module.DeleteSync.Helpers;
using Gigya.Module.DeleteSync.Models;
using Gigya.Module.DeleteSync.Tests.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gigya.Module.DeleteSync.Tests.EmailTests
{
    [TestClass]
    public class EmailTests
    {
        [TestMethod]
        public void TestConfirmationEmail()
        {
            var provider = new FakeEmailProvider();
            var helper = new EmailHelper(provider);

            var model = new DeleteSyncEmailModel
            {
                From = "no-reply@sample.com",
                To = "sample@sample.com",
                Domain = "localhost",
                DateCompleted = DateTime.Now,
                DateStarted = DateTime.Now.AddMinutes(-5),
                DeletedUids = new List<string> { "deletedUid", "anotherDeletedUid" },
                UpdatedUids = new List<string> { "updatedUid", "anotherUpdateUid", "aThirdUpdatedUid" },
                FailedDeletedUids = new List<string> { "thisOneFailed" },
                FailedUpdatedUids = new List<string> { "anotherFailureThatSucks" },
                ProcessedFilenames = new List<string> { "file1.csv", "file2.csv", "file3.csv" },
                Subject = "test"
            };
            helper.SendConfirmation(model);
        }
    }
}
