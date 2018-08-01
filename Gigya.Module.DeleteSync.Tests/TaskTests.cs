using System;
using System.Collections.Generic;
using System.Linq;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.DeleteSync.Models;
using Gigya.Module.DeleteSync.Providers;
using Gigya.Module.DeleteSync.Tests.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gigya.Module.DeleteSync.Tests
{
    [TestClass]
    public class TaskTests
    {
        private const string _accessKey = "AKIAJDZFARTEO5I6LDTQ";
        private const string _secretKey = "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";
        private const string _bucketName = "lpsgigya";
        private const string _prefix = "";
        private const string _region = "eu-west-2";
        private Logger _logger = new Logger(new FakeCmsLogger());

        [TestMethod]
        public void CanGetS3Files()
        {
            var provider = new AmazonProvider(_accessKey, _secretKey, _bucketName, _prefix, _region, _logger);

            Dictionary<string, DeleteSyncLog> processedFiles = new Dictionary<string, DeleteSyncLog>();
            var result = provider.GetUids(processedFiles).Result;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
        }

        [TestMethod]
        public void UIDsAreValid()
        {
            var provider = new AmazonProvider(_accessKey, _secretKey, _bucketName, _prefix, _region, _logger);

            Dictionary<string, DeleteSyncLog> processedFiles = new Dictionary<string, DeleteSyncLog>();
            var result = provider.GetUids(processedFiles).Result;

            Assert.IsNotNull(result);
            Assert.IsTrue(!result.Any(i => string.IsNullOrEmpty(i.Key)));
            Assert.IsTrue(result.Any(i => i.UIDs.Any()));
            Assert.IsTrue(!result.Any(i => i.UIDs.Any(j => string.IsNullOrEmpty(j))));
        }

        //[TestMethod]
        //public void ProcessedFileIsntReturned()
        //{
        //    var provider = new AmazonProvider(_accessKey, _secretKey, _bucketName, _prefix, _logger);

        //    Dictionary<string, DeleteSyncLog> processedFiles = new Dictionary<string, DeleteSyncLog>();
        //    processedFiles.Add("test", new DeleteSyncLog
        //    {
        //        Key = "test",
        //        Total = 10,
        //        Success = 10
        //    });

        //    var result = provider.GetUids(processedFiles).Result;

        //    Assert.IsNotNull(result);
        //    Assert.IsTrue(result.Any());
        //}
    }
}
