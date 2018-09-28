using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Gigya.Module.Core.Connector.Common;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Gigya.UnitTests
{
    /// <summary>
    /// Summary description for UtilsTests
    /// </summary>
    [TestClass]
    public class UtilsTests
    {
        private const string _json = @"{
              ""socialProviders"": ""site"",
              ""data"": {
                    ""phoneNumbers"": [
                        {
                            ""phone"": 1234
                        },
                        {   
                            ""phone"": 4567
                        }
                    ],
                    ""phoneNumbers2"": [
                        ""0123456789"",
                        ""1111111111""
                    ]
              },
              ""phoneNumbers"": [
                    {
                        ""phone"": 1234
                    },
                    {   
                        ""phone"": 4567
                    }
                ],
              ""profile"": {
                ""firstName"": ""Jason"",
                ""age"": 1
              }
            }";

        private const string _json2 = @"{
              ""profile2"": {
                ""firstName"": ""new"",
                ""age"": 1
              }
            }";

        private const string _json3 = @"{
              ""profile"": {
                ""firstName"": ""updated3"",
                ""age"": 1
              }
            }";

        private const string _jsonProfileAddress = @"{
              ""profile"": {
                ""address"": {
                    ""line1"": ""line 1""
                }
              }
            }";

        private const string _jsonProfileAddress2 = @"{
              ""profile"": {
                ""address"": {
                    ""line2"": ""line 2""
                }
              }
            }";

        [TestMethod]
        public void CanGetNestedPropertyFromDynamic()
        {
            dynamic data = JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(_json);

            var firstName = DynamicUtils.GetValue<string>(data, "profile.firstName");

            Assert.IsNotNull(firstName, "Value not found");
            Assert.AreEqual(firstName, "Jason", "Value doesn't match");
        }

        [TestMethod]
        public void CanGetNestedLongPropertyFromDynamic()
        {
            dynamic data = JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(_json);

            var value = DynamicUtils.GetValue<long>(data, "profile.age");

            Assert.IsNotNull(value, "Value not found");
            Assert.AreEqual(value, 1, "Value doesn't match");
        }

        [TestMethod]
        public void CanGetPropertyFromDynamic()
        {
            dynamic data = JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(_json);

            var value = DynamicUtils.GetValue<string>(data, "socialProviders");

            Assert.IsNotNull(value, "Value not found");
            Assert.AreEqual(value, "site", "Value doesn't match");
        }

        [TestMethod]
        public void CanGetArrayPropertyFromDynamic()
        {
            dynamic data = JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(_json);

            var value = DynamicUtils.GetValue<long>(data, "phoneNumbers[0].phone");

            Assert.IsNotNull(value, "Value not found");
            Assert.AreEqual(1234, value, "Value doesn't match");
        }

        [TestMethod]
        public void CanGetNestedArrayPropertyFromDynamic()
        {
            dynamic data = JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(_json);

            var value = DynamicUtils.GetValue<long>(data, "data.phoneNumbers[1].phone");

            Assert.IsNotNull(value, "Value not found");
            Assert.AreEqual(4567, value, "Value doesn't match");
        }

        [TestMethod]
        public void CanGetNestedArrayPropertyFromDynamic2()
        {
            dynamic data = JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(_json);

            var value = DynamicUtils.GetValue<string>(data, "data.phoneNumbers2[1]");

            Assert.IsNotNull(value, "Value not found");
            Assert.AreEqual("1111111111", value, "Value doesn't match");

            value = DynamicUtils.GetValue<string>(data, "data.phoneNumbers2[0]");

            Assert.IsNotNull(value, "Value not found");
            Assert.AreEqual("0123456789", value, "Value doesn't match");
        }

        [TestMethod]
        public void DefaultValueReturnedIfInvalidProperty()
        {
            dynamic data = JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(_json);

            var value = DynamicUtils.GetValue<long>(data, "data.phoneNumbers.phone");
            Assert.AreEqual(0, value);
        }

        [TestMethod]
        public void DefaultValueReturnedIfInvalidArrayIndex()
        {
            dynamic data = JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(_json);

            var value = DynamicUtils.GetValue<long>(data, "data.phoneNumbers[2].phone");
            Assert.AreEqual(0, value);
        }

        //[TestMethod]
        public void SpeedTestForArrayPropertyFromDynamic()
        {
            dynamic data = JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(_json);

            Gigya.Module.Core.Connector.Common.DynamicUtils.GetValue<long>(data, "profile.age");
            DynamicUtils.GetValue<long>(data, "profile.age");

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < 20; i++)
            {
                var value = Gigya.Module.Core.Connector.Common.DynamicUtils.GetValue<long>(data, "profile.age");
            }

            var oldVersionMs = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();

            for (int i = 0; i < 20; i++)
            {
                var value = DynamicUtils.GetValue<long>(data, "profile.age");
            }

            var newVersionMs = stopwatch.ElapsedMilliseconds;

            Assert.Inconclusive(string.Format("Old version: {0}, New version: {1}", oldVersionMs, newVersionMs));
        }

        [TestMethod]
        public void CanMergeJson()
        {
            dynamic data1 = JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(_json);
            dynamic data2 = JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(_json2);

            var result = DynamicUtils.Merge(data1, data2);

            Assert.IsNotNull(result);

            Assert.AreEqual(result.profile2.firstName, "new", "profile2.firstName doesn't contain expected values.");

            // check old properties remain
            Assert.AreEqual(result.profile.firstName, "Jason", "profile.firstName doesn't contain expected values.");
        }

        [TestMethod]
        public void CanMergeNestedJson()
        {
            dynamic data1 = JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(_jsonProfileAddress);
            dynamic data2 = JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(_jsonProfileAddress2);

            var result = DynamicUtils.Merge(data1, data2);
            
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.profile);
            Assert.AreEqual(result.profile.address.line1, "line 1", "profile.address.line1 doesn't contain expected values.");

            Assert.AreEqual(result.profile.address.line2, "line 2", "profile.address.line2 doesn't contain expected values.");
        }

        [TestMethod]
        public void MergeSameProperty()
        {
            dynamic data1 = JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(@"{
              ""profile"": {
                ""firstName"": ""original"",
                ""age"": 1
              }
            }");
            dynamic data2 = JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(@"{
              ""profile"": {
                ""firstName"": ""new"",
                ""age"": 1
              }
            }");

            var result = DynamicUtils.Merge(data1, data2);
            
            Assert.AreEqual(result.profile.firstName, "new");
        }

        [TestMethod]
        public void MergeProfile2()
        {
            dynamic data1 = JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(@"{
              ""profile"": {
                ""firstName"": ""original"",
                ""age"": 1
              }
            }");
            dynamic data2 = JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(@"{
              ""profile2"": {
                ""firstName"": ""new"",
                ""age"": 1
              }
            }");

            var result = DynamicUtils.Merge(data1, data2);

            Assert.AreEqual(result.profile.firstName, "original");
            Assert.AreEqual(result.profile2.firstName, "new");
        }
    }
}
