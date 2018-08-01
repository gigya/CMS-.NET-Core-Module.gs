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
        private const string _accessKey = "";
        private const string _secretKey = "";
        private const string _bucketName = "";
        private const string _prefix = "";
        private Logger _logger = new Logger(new FakeCmsLogger());

        [TestMethod]
        public void CanGetS3Files()
        {
            var provider = new AmazonProvider(_accessKey, _secretKey, _bucketName, _prefix, _logger);

            Dictionary<string, DeleteSyncLog> processedFiles = new Dictionary<string, DeleteSyncLog>();
            var result = provider.GetUids(processedFiles).Result;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
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
