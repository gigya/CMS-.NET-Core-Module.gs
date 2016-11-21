using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Text;
using OpenQA.Selenium.Firefox;
using System.Configuration;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Remote;
using Newtonsoft.Json;
using System.Collections.Generic;
using Gigya.Module.Core.Connector.Models;
using System.Xml;
using System.IO;
using System.Data.SqlClient;

namespace Gigya.UnitTests.Selenium.Authentication
{
    [TestClass]
    public class SitefinityRegisterLoginTests : RegisterLoginTests
    {
        private IWebDriver _sitefinityAdminDriver;
        
        [TestCleanup]
        public void TestCleanUp()
        {
            base.TeardownTest();

            try
            {
                if (_sitefinityAdminDriver != null)
                {
                    _sitefinityAdminDriver.Quit();
                }
            }
            catch
            {
                // Ignore errors if unable to close the browser
            }
        }

        [TestMethod]
        public void Sitefinity_CanRegisterAndLoginToCms()
        {
            base.CanRegisterAndLoginToCms();
        }

        [TestMethod]
        public void Sitefinity_IsLoggedIntoSecondSiteAsWell()
        {
            base.IsLoggedIntoSecondSiteAsWell();
        }

        //[TestMethod]
        //public void Sitefinity_CmsExceptionLogsInUser()
        //{
        //    base.CmsExceptionLogsInUser();
        //}

        [TestMethod]
        public void Sitefinity_CanLogoutAndIsLoggedOutOfSecondSiteAsWell()
        {
            base.CanLogoutAndIsLoggedOutOfSecondSiteAsWell();
        }

        [TestMethod]
        public void Sitefinity_CanUpdateProfileSitefinity()
        {
            if (!_loggedIn)
            {
                CanRegisterAndLoginToCms();
            }

            CanLogoutAndIsLoggedOutOfSecondSiteAsWell();

            CanLoginToFrontEnd();

            var button = _driver.FindElement(By.CssSelector(".gigya-edit-profile"), 5);
            if (button != null)
            {
                button.Click();
            }

            // wait for form
            var form = _driver.FindElement(By.Id("gigya-profile-form"), 5);

            _newEmail = string.Concat(Guid.NewGuid(), "@purestone.co.uk");

            form.FindElement(By.Name("profile.email")).ClearAndSendKeys(_newEmail);
            form.FindElement(By.Name("profile.firstName")).ClearAndSendKeys(_updatedFirstName);
            form.FindElement(By.Name("profile.lastName")).ClearAndSendKeys(_updatedLastName);
            form.FindElement(By.CssSelector("input.gigya-input-submit")).Click();

            LoginToSitefinityAdmin();

            // check profile has been updated in Sitefinity
            _sitefinityAdminDriver.Navigate().GoToUrl(Config.Site1BaseURL + "Sitefinity/Administration/Users");
            Thread.Sleep(10000);

            // find user
            var searchButton = _sitefinityAdminDriver.FindElement(By.Id("findUsersLink"), 10);
            if (!searchButton.Displayed)
            {
                Thread.Sleep(10000);
            }
            searchButton.Click();

            var searchType = new SelectElement(_sitefinityAdminDriver.FindElement(By.Id("searchType")));
            searchType.SelectByValue("Email");

            _sitefinityAdminDriver.FindElement(By.Id("filterText")).SendKeys(_newEmail);
            _sitefinityAdminDriver.FindElement(By.Id("A1")).Click();

            // wait for results
            var userCell = _sitefinityAdminDriver.FindElement(By.CssSelector(".sfUserCell"), 10);
            userCell.FindElement(By.ClassName("editCommand")).Click();


            // wait for user details
            Thread.Sleep(1000);
            _sitefinityAdminDriver.SwitchTo().Frame("editUserDialog");

            ValidateUserField("Email", _newEmail);
            ValidateUserField("First name", _updatedFirstName);
            ValidateUserField("Last name", _updatedLastName);
        }

        private void ValidateUserField(string labelName, string requiredValue)
        {
            var label = _sitefinityAdminDriver.FindElement(By.XPath("//*[text() = '" + labelName + "']"), 25);
            var input = _sitefinityAdminDriver.FindElement(By.Id(label.GetAttribute("for")));

            Assert.AreEqual(requiredValue, input.GetAttribute("value"), string.Format("Value for {0} didn't match the required value of {1}.", labelName, requiredValue));
        }

        private void LoginToSitefinityAdmin(int timeout = 0)
        {
            if (_sitefinityAdminDriver != null)
            {
                return;
            }

            _sitefinityAdminDriver = new FirefoxDriver();

            SitefinityUtils.LoginToSitefinity(_sitefinityAdminDriver, timeout);
        }

        private void UpdateGlobalParams(string json, int loginTimeout = 0)
        {
            LoginToSitefinityAdmin(loginTimeout);

            _sitefinityAdminDriver.Navigate().GoToUrl(Config.Site1BaseURL + "Sitefinity/Administration/Settings/Basic/GigyaModule/");

            _sitefinityAdminDriver.FindElementFromLabel("Global Parameters (JSON)", 5).ClearWithBackspaceAndSendKeys(json);
            _sitefinityAdminDriver.FindElement(By.ClassName("sfSave"), 5).Click();

            var positiveMessage = _sitefinityAdminDriver.FindElement(By.ClassName("sfMsgPositive"), 2);
            Assert.IsNotNull(positiveMessage, "Settings not saved. Check they are correct.");
        }

        private void UpdateAllGlobalParams(string json)
        {
            // update all gigya settings to have a session expiration of 10 seconds
            UpdateGlobalParams(json, 300);

            _sitefinityAdminDriver.FindElement(By.CssSelector(".sfSiteSelectorMenuWrp .clickMenu .main"), 5).Click();
            Thread.Sleep(1000);

            var sites = _sitefinityAdminDriver.FindElements(By.CssSelector(".clickMenu .sfSiteLink"));
            sites.First(i => !i.GetAttribute("className").Contains("sfSel")).Click();

            Thread.Sleep(1000);

            UpdateGlobalParams(json);
        }

        [TestMethod]
        public void Sitefinity_IsSessionExpiredCorrectly()
        {
            // ensure caching is turned off
            XmlDocument sysConfigDoc = new XmlDocument();
            var path = Path.Combine(Config.SitefinityRootPath, "App_Data", "Sitefinity", "Configuration", "SystemConfig.config");
            sysConfigDoc.Load(path);
            
            // disable caching
            var outputCacheElem = sysConfigDoc.SelectSingleNode("/systemConfig/outputCacheSettings");
            if (outputCacheElem == null)
            {
                var sysConfig = sysConfigDoc.SelectSingleNode("/systemConfig");

                outputCacheElem = sysConfigDoc.CreateElement("outputCacheSettings");
                var attribute = sysConfigDoc.CreateAttribute("enableOutputCache");
                attribute.Value = "False";

                outputCacheElem.Attributes.Append(attribute);

                sysConfig.AppendChild(outputCacheElem);

                sysConfigDoc.Save(path);

                // restart site by updating web.config
                XmlDocument webConfigDoc = new XmlDocument();
                webConfigDoc.Load(Path.Combine(Config.SitefinityRootPath, "web.config"));
                webConfigDoc.Save(Path.Combine(Config.SitefinityRootPath, "web.config"));
            }

            UpdateAllGlobalParams("{ \"sessionExpiration\": 10 }");

            var reset = false;

            try
            {
                // test session expiration works now that we know all sites are using 10 second timeout
                CanRegisterAndLoginToCms();

                IsLoggedIntoSecondSiteAsWell();

                _driver.Navigate().GoToUrl(Config.Site1BaseURL);

                // wait for session to expire
                Thread.Sleep(11000);
                HasSessionExpired();

                // check logged out of site 2 as well
                _driver.Navigate().GoToUrl(Config.Site2BaseURL);
                HasSessionExpired();

                // login again
                CanLoginToFrontEnd();

                UpdateAllGlobalParams(string.Empty);
                reset = true;
            }
            catch
            {
                if (!reset)
                {
                    UpdateAllGlobalParams(string.Empty);
                }
                throw;
            }
        }

        //[TestMethod]
        public void Sitefinity_IsUserLoggedInAfterSessionExpired()
        {
            LoginToSitefinityAdmin();
            SetAuthTimeout(1);

            // update all gigya settings to have a session expiration of 500 seconds
            UpdateAllGlobalParams("{ \"sessionExpiration\": 500 }");

            var reset = false;

            try
            {
                // test session expiration works now that we know all sites are using 500 second timeout
                CanRegisterAndLoginToCms();

                IsLoggedIntoSecondSiteAsWell();

                _driver.Navigate().GoToUrl(Config.Site1BaseURL);

                // wait for cms session to expire
                Thread.Sleep(70000);

                // refresh page and should now be logged out as cms session is set to 1 min
                _driver.Navigate().Refresh();

                // user should now be logged out
                var loginButton = _driver.FindElement(By.ClassName("gigya-login"), 10);
                Assert.IsNotNull(loginButton, "Login button not found. User should be logged out.");

                Thread.Sleep(5000);

                // refresh page and should now be logged in as gigya session timeout is more than cms
                _driver.Navigate().Refresh();

                var logoutButton = _driver.FindElement(By.ClassName("gigya-logout"), 10);
                Assert.IsNotNull(logoutButton, "Logout button not found. User should be logged in.");

                UpdateAllGlobalParams(string.Empty);

                // reset
                SetAuthTimeout(600);

                reset = true;
            }
            catch
            {
                if (!reset)
                {
                    UpdateAllGlobalParams(string.Empty);

                    // reset
                    SetAuthTimeout(600);
                }
                throw;
            }
        }

        private void SetAuthTimeout(int timeout)
        {
            _sitefinityAdminDriver.Navigate().GoToUrl(Config.Site1BaseURL + "Sitefinity/Administration/Settings/Advanced");
            _sitefinityAdminDriver.FindElement(By.XPath("//*[contains(text(), 'Security')]"), 1).Click();

            Thread.Sleep(1000);

            _sitefinityAdminDriver.FindElementFromLabel("AuthCookieTimeout", 1).ClearAndSendKeys(timeout.ToString());

            _sitefinityAdminDriver.FindElement(By.PartialLinkText("Save changes")).Click();

            Thread.Sleep(5000);
        }

        [TestMethod]
        public void Sitefinity_RemoveCookieAndUserIsLoggedInAfterRefresh()
        {
            CanRegisterAndLoginToCms();
            LogUserOutOfSitefinity();

            _driver.Navigate().Refresh();

            // user should be logged out as cookie deleted
            var button = _driver.FindElement(By.CssSelector(".gigya-login"), 1);
            if (button == null)
            {
                var loginScreen = _driver.FindElement(By.Id("gigya-login-screen"), 1);
                Assert.IsNotNull(loginScreen, "Login button not found. User should be logged out.");
            }

            Thread.Sleep(5000);

            _driver.Navigate().Refresh();
            var logoutButton = _driver.FindElement(By.ClassName("gigya-logout"), 10);
            Assert.IsNotNull(logoutButton, "Logout button not found. User should be logged in.");
        }

        private void LogUserOutOfSitefinity()
        {
            // simulate the user being logged out of Sitefinity
            _driver.Manage().Cookies.DeleteCookieNamed("SF-TokenId");
            _driver.Manage().Cookies.DeleteCookieNamed("FedAuth");
        }

        [TestMethod]
        public void Sitefinity_IsLoggedIntoGigyaAfterTimeout_SitefinitySessionProvider()
        {
            // update all gigya settings to have a session expiration of 60 seconds
            UpdateAllGlobalParams("{ \"sessionExpiration\": 30 }");

            // login to CMS
            CanRegisterAndLoginToCms();

            // wait 40 secs - longer than gigya timeout
            Thread.Sleep(40000);

            _driver.Navigate().Refresh();

            // check that user is logged into Gigya
            var cookies = _driver.Manage().Cookies.AllCookies;
            if (!cookies.Any(i => i.Name == ("glt_" + Config.Site1ApiKey.Trim())))
            {
                Assert.Fail("Failed to find Gigya cookie starting with glt");
            }

            Thread.Sleep(5000);
        }

        [TestMethod]
        public void Sitefinity_IsLoggedOutOfGigyaAfterTimeout_SitefinitySessionProvider()
        {
            UpdateAllGlobalParams("{ \"sessionExpiration\": 300 }");

            // login to CMS
            CanRegisterAndLoginToCms();

            // remove sitefinity cookie - effectively log the user out
            LogUserOutOfSitefinity();

            _driver.Navigate().Refresh();

            Thread.Sleep(5000);

            // check that user is logged out of Gigya
            var cookies = _driver.Manage().Cookies.AllCookies;
            if (cookies.Any(i => i.Name.StartsWith("glt")))
            {
                Assert.Fail("Found Gigya cookie starting with glt. User should be logged out of Gigya");
            }

            Thread.Sleep(5000);
        }
    }
}
