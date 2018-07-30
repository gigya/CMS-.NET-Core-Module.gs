using System;
using System.Linq;
using Gigya.Module.Core.Connector.Logging;
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

            var result = provider.GetUids().Result;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
        }
    }
}
