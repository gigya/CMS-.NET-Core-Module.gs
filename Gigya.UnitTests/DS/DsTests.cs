using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using Newtonsoft.Json;
using Gigya.Module.DS.Helpers;
using Gigya.Module.Core.Connector.Logging;
using Gigya.UnitTests.Logging;
using Gigya.Module.Core.Data;
using Gigya.Module.Core.Connector.Models;
using System.Linq;
using System.Dynamic;
using Moq;
using Gigya.Module.DS.Config;

namespace Gigya.UnitTests.DS
{
    /// <summary>
    /// Summary description for DsTests
    /// </summary>
    [TestClass]
    public class DsTests
    {
        private static readonly string _dsBaseUrl = "https://ds.eu1.gigya.com/";
        private static readonly string _dsUid = "271945af9f6b47119e883dc17167aead";
        private static readonly string _dsSecret = "rH7ZVYbTaodksq6u/JPI6OBe/rT/IZmN";
        private static readonly string _dsUserKey = "ABPcVRLxt+1u";
        private static readonly string _dsApiKey = "3_qkAT5OcGyvYpkjc_VF6-OfoeTKGk4T_jVwjFF9f5TQzoAg-mH8SBsjQi1srdsOm6";
        private static readonly string _dsQuery = "SELECT * FROM {0} WHERE UID = '{1}'";
        private static readonly string _dsType = "dsTesting";
        private static readonly string _dsOid = "test";
        private static readonly string _dsData = @"{
           ""firstName_s"" : ""david"",
           ""birthDate_d"" : ""1978-07-22"",
           ""about_t"" : ""I love tracking,..."",
           ""age_i"": 30,
           ""academicDegree"" : {
               ""university_s"": ""Berkeley"",
               ""degree_s"": ""MBA"",
               ""department_s"": ""Business""
           }
        }";
        //private GigyaDsSettings _dsSettings;

        private Logger _logger = new Logger(new FakeCmsLogger());

        [TestInitialize]
        public void Init()
        {
            if (!CheckDummyDsData())
            {
                CreateDummyDsData();
            }

            //_dsSettings = new GigyaDsSettings
            //{
            //    MappingsByType = new Dictionary<string, List<GigyaDsMapping>>()
            //};

            //_dsSettings.MappingsByType.Add("userinfo", new List<GigyaDsMapping>
            //{
            //    new GigyaDsMapping
            //    {
            //        CmsName = "test",
            //        GigyaDsType = "dsTesting",
            //        Custom = new Custom
            //        {
            //            Oid = "test"
            //        },
            //        GigyaFieldName = "firstName_s",
            //        GigyaName = "ds.test.firstName_s"
            //    }
            //});
        }

        private bool CheckDummyDsData()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_dsBaseUrl);

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("UID", _dsUid),
                new KeyValuePair<string, string>("secret", _dsSecret),
                new KeyValuePair<string, string>("userKey", _dsUserKey),
                new KeyValuePair<string, string>("apiKey", _dsApiKey),
                new KeyValuePair<string, string>("query", string.Format(_dsQuery, _dsType, _dsUid))
            });

            var response = httpClient.PostAsync("ds.search", content).Result;
            var responseData = response.Content.ReadAsStringAsync().Result;
            var responseModel = JsonConvert.DeserializeObject<dynamic>(responseData);
            if (responseModel.statusCode.Value != 200 || responseModel.results.Count == 0)
            {
                return false;
            }

            return true;
        }
        
        public void CreateDummyDsData()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_dsBaseUrl);

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("UID", _dsUid),
                new KeyValuePair<string, string>("secret", _dsSecret),
                new KeyValuePair<string, string>("apiKey", _dsApiKey),
                new KeyValuePair<string, string>("userKey", _dsUserKey),
                new KeyValuePair<string, string>("type", _dsType),
                new KeyValuePair<string, string>("oid", _dsOid),
                new KeyValuePair<string, string>("data", _dsData)
            });

            var response = httpClient.PostAsync("ds.store", content).Result;
            var responseData = response.Content.ReadAsStringAsync().Result;
            var responseModel = JsonConvert.DeserializeObject<dynamic>(responseData);
            Assert.AreEqual(200, responseModel.statusCode.Value);
        }

        [TestMethod]
        public void DsDataMergedWithAccountInfo()
        {
            var settingsHelper = new GigyaDsSettingsHelper(_logger);
            var settings = settingsHelper.Get("-1");

            settings.Method = GigyaDsMethod.Get;
            TestDsDataRetrieved(settings);

            settings.Method = GigyaDsMethod.Search;
            TestDsDataRetrieved(settings);
        }

        private void TestDsDataRetrieved(GigyaDsSettings dsSettings)
        {
            dynamic gigyaModel = new ExpandoObject();
            gigyaModel.UID = _dsUid;

            var settings = new GigyaModuleSettings
            {
                Id = -1,
                ApiKey = _dsApiKey,
                ApplicationKey = _dsUserKey,
                ApplicationSecret = _dsSecret,
                MappingFields = string.Empty,
                DataCenter = "eu1"
            };

            //var settingsHelper = new Mock<GigyaDsSettingsHelper>(_logger);
            //settingsHelper.Setup(i => i.Get(It.IsAny<string>())).Returns(dsSettings);

            var mappingFields = new List<MappingField>();

            var helper = new GigyaDsHelper(settings, _logger, dsSettings);
            var result = helper.Merge(gigyaModel, mappingFields);

            Assert.IsNotNull(result);
            Assert.IsTrue(mappingFields.Any(), "Mapping fields not added");

            // verify all the fields
            Assert.AreEqual(30, result.ds.dsTesting.age_i);
            Assert.AreEqual("david", result.ds.dsTesting.firstName_s);
            Assert.AreEqual("1978-07-22", result.ds.dsTesting.birthDate_d);
            Assert.AreEqual("MBA", result.ds.dsTesting.academicDegree.degree_s);
        }

        [TestMethod]
        public void TestInvalidUidReturnsNull()
        {
            var settingsHelper = new GigyaDsSettingsHelper(_logger);
            var settings = settingsHelper.Get("-1");

            TestInvalidUidReturnsNull_Get(settings);
            TestInvalidUidReturnsNull_Search(settings);
        }

        private void TestInvalidUidReturnsNull_Get(GigyaDsSettings dsSettings)
        {
            var settings = new GigyaModuleSettings
            {
                Id = -1,
                ApiKey = "3_qkAT5OcGyvYpkjc_VF6-OfoeTKGk4T_jVwjFF9f5TQzoAg-mH8SBsjQi1srdsOm6",
                MappingFields = string.Empty,
                DataCenter = "eu1"
            };

            var helper = new GigyaDsHelper(settings, _logger, dsSettings);
            var result = helper.GetAll("abc");

            Assert.IsNull(result);
        }
        
        private void TestInvalidUidReturnsNull_Search(GigyaDsSettings dsSettings)
        {
            var settings = new GigyaModuleSettings
            {
                Id = -1,
                ApiKey = "3_qkAT5OcGyvYpkjc_VF6-OfoeTKGk4T_jVwjFF9f5TQzoAg-mH8SBsjQi1srdsOm6",
                MappingFields = string.Empty,
                DataCenter = "eu1"
            };

            var helper = new GigyaDsHelper(settings, _logger, dsSettings);
            var result = helper.SearchAll("abc");

            Assert.IsNull(result);
        }
    }
}
