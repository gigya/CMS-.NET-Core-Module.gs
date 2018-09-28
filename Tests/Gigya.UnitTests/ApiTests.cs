//using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Gigya.Module.Core.Connector.Helpers;
//using Gigya.UnitTests.Logging;
//using Gigya.Module.Core.Connector.Logging;
//using Gigya.Module.Core.Data;
//using SCG = Sitecore.Gigya.Module;
//using System.Linq;

//namespace Gigya.UnitTests
//{
//    [TestClass]
//    public class ApiTests
//    {
//        [TestMethod]
//        public void CanGetAccountInfo()
//        {
//            //prod: var userId = "4a424531b1a24dadb114349238a07df7";
//            var userId = "271945af9f6b47119e883dc17167aead";

//            var settingsHelper = new Gigya.Umbraco.Module.Helpers.GigyaSettingsHelper();
//            var logger = new Logger(new FakeCmsLogger());
//            var apiHelper = new GigyaApiHelper(settingsHelper, logger);

//            var settings = Settings<GigyaModuleSettings>();
//            var response = apiHelper.GetAccountInfo(userId, settings);
//            if (response == null || response.GetErrorCode() != 0)
//            {
//                Assert.Fail("Invalid response");
//            }
//        }

//        private T Settings<T>() where T: GigyaModuleSettings, new()
//        {
//            return new T
//            {
//                ApiKey = "3_qkAT5OcGyvYpkjc_VF6-OfoeTKGk4T_jVwjFF9f5TQzoAg-mH8SBsjQi1srdsOm6",
//                ApplicationKey = "ABPcVRLxt+1u",
//                ApplicationSecret = "rH7ZVYbTaodksq6u/JPI6OBe/rT/IZmN",
//                Language = "auto",
//                LanguageFallback = "en",
//                DebugMode = true,
//                DataCenter = "eu1.gigya.com",
//                EnableRaas = true
//            };
//        }

//        //private GigyaModuleSettings Settings()
//        //{
//        //    return new GigyaModuleSettings
//        //    {
//        //        ApiKey = "3_SIW9QpnKPCQQaTzf-ENLmw8Ou-I0s4IZlK5AGRWE-Q7UQ03VTJO8dNx8oLykUBfO",
//        //        ApplicationKey = "ADaWitOVBHNW",
//        //        ApplicationSecret = "uatkwf/+53cxfsOHWO4JiZomotnOglCl",
//        //        Language = "auto",
//        //        LanguageFallback = "en",
//        //        DebugMode = true,
//        //        DataCenter = "us1.gigya.com",
//        //        EnableRaas = true
//        //    };
//        //}
//    }
//}
