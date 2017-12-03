using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gigya.UnitTests.Selenium.Authentication;
using System.Threading;
using OpenQA.Selenium;

namespace Gigya.UnitTests.Selenium
{
    [TestClass]
    public class DynamicSessionTests : SitefinityRegisterLoginTests
    {
        [TestMethod]
        public void IsSessionDynamicallyExtendedSingleSite()
        {
            // login to front end
            _newEmail = "jason.king@purestone.co.uk";
            _passsword = "aa234567";

            CanLoginToFrontEnd();

            // wait 70 seconds
            Thread.Sleep(70000);

            // refresh browser
            _driver.Navigate().Refresh();

            // should still be logged in
            var logoutButton = _driver.FindElement(By.ClassName("gigya-logout"), 10);
            Assert.IsNotNull(logoutButton, "Logout button not found. User should be logged in.");
        }

        /// <summary>
        /// Site should be configured to have a session timeout of 120 seconds.
        /// </summary>
        [TestMethod]
        public void IsSessionDynamicallyExtendedSso()
        {
            // login to front end
            _newEmail = "jason.king@purestone.co.uk";
            _passsword = "aa234567";

            CanLoginToFrontEnd();

            IsLoggedIntoSecondSiteAsWell();

            // wait 70 seconds
            Thread.Sleep(70000);

            // refresh browser
            _driver.Navigate().GoToUrl(Config.Site1BaseURL);

            // should still be logged in
            var logoutButton = _driver.FindElement(By.ClassName("gigya-logout"), 10);
            Assert.IsNotNull(logoutButton, "Logout button not found. User should be logged in.");

            // should also be logged into second site as well
            IsLoggedIntoSecondSiteAsWell();
        }

        /// <summary>
        /// Site should be configured to have a session timeout of 120 seconds.
        /// </summary>
        [TestMethod]
        public void IsSessionDynamicallyExtendedAndExpired()
        {
            // login to front end
            _newEmail = "jason.king@purestone.co.uk";
            _passsword = "aa234567";

            CanLoginToFrontEnd();

            // wait 160 seconds
            Thread.Sleep(160000);

            // refresh browser
            _driver.Navigate().GoToUrl(Config.Site1BaseURL);
            Thread.Sleep(10000);

            _driver.Navigate().Refresh();

            Thread.Sleep(10000);

            // should be logged out
            var logoutButton = _driver.FindElement(By.ClassName("gigya-logout"), 10);
            Assert.IsNull(logoutButton, "Logout button found. User should not be logged in.");
        }

        /// <summary>
        /// Site should be configured to have a session timeout of 120 seconds.
        /// </summary>
        [TestMethod]
        public void IsSessionDynamicallyExtendedAndExpiredSite2()
        {
            // login to front end
            _newEmail = "jason.king@purestone.co.uk";
            _passsword = "aa234567";

            CanLoginToFrontEnd();

            // wait 160 seconds
            Thread.Sleep(160000);

            // refresh browser
            _driver.Navigate().GoToUrl(Config.Site2BaseURL);
            Thread.Sleep(10000);

            _driver.Navigate().Refresh();

            Thread.Sleep(10000);

            // should be logged out
            var logoutButton = _driver.FindElement(By.ClassName("gigya-logout"), 10);
            Assert.IsNull(logoutButton, "Logout button found. User should not be logged in.");
        }

        [TestMethod]
        public void GigyaUidTest()
        {
            var uidKeyValue = "UUID=b2f5dda5de6046c5ab870c7376641ab8";
            var uid = uidKeyValue.Substring(5, 32);

            Assert.AreEqual("b2f5dda5de6046c5ab870c7376641ab8", uid);
        }
    }
}
