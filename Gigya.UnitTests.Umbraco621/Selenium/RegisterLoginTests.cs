﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Text;
using OpenQA.Selenium.Firefox;
using System.Configuration;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Remote;
using Umbraco.Core;
using Umbraco.Core.Services;
using Umbraco.Core.Persistence;
using Gigya.UnitTests.Umbraco;
using Gigya.Umbraco.Module.v621.Data;

namespace Gigya.UnitTests.Selenium
{
    [TestClass]
    public class RegisterLoginTests
    {
        private IWebDriver _driver;

        private const string _passsword = "aa234567";
        private const string _firstName = "Mr";
        private const string _lastName = "Tester";
        
        private const string _updatedFirstName = "Mickey";
        private const string _updatedLastName = "Mouse";

        private bool _loggedIn;

        private string _newEmail;

        [TestInitialize]
        public void SetupTest()
        {
            _driver = new FirefoxDriver();
            _newEmail = string.Concat(Guid.NewGuid(), "@purestone.co.uk");
        }

        [TestCleanup]
        public void TeardownTest()
        {
            try
            {
                _driver.Quit();
            }
            catch
            {
                // Ignore errors if unable to close the browser
            }
        }

        [TestMethod]
        public void CanRegisterAndLoginToUmbraco621()
        {
            _driver.Navigate().GoToUrl(Config.Site1BaseURL);

            // wait the usual 3 mins for sitefinity to start up....
            _driver.FindElement(By.CssSelector(".gigya-register"), 180).Click();
            
            var dialog = _driver.FindElement(By.Id("gigya-register-screen"), 2);
            dialog.FindElement(By.Name("email")).SendKeys(_newEmail);

            dialog.FindElement(By.Name("profile.firstName")).SendKeys(_firstName);
            dialog.FindElement(By.Name("profile.lastName")).SendKeys(_lastName);
            dialog.FindElement(By.Name("password")).SendKeys(_passsword);
            dialog.FindElement(By.Name("passwordRetype")).SendKeys(_passsword);
            dialog.FindElement(By.CssSelector("input.gigya-input-submit")).Click();

            // wait up to 30 seconds for reload
            var logoutButton = _driver.FindElement(By.ClassName("gigya-logout"), 10);
            Assert.IsNotNull(logoutButton, "Logout button not found. User should be logged in.");

            _loggedIn = true;
        }

        [TestMethod]
        public void SitefinityExceptionLogsInUser()
        {
            _newEmail = Gigya.Module.Core.Constants.Testing.EmailWhichThrowsException;

            _driver.Navigate().GoToUrl(Config.Site1BaseURL);

            // wait the usual 3 mins for sitefinity to start up....
            _driver.FindElement(By.CssSelector(".gigya-register"), 180).Click();

            var dialog = _driver.FindElement(By.Id("gigya-register-screen"), 2);
            var email = dialog.FindElement(By.Name("email"));
            email.SendKeys(_newEmail);

            // wait for unique email validation to complete
            Thread.Sleep(5000);

            // see if email already exists
            var emailError = email.FindElement(By.XPath("following-sibling::*"));
            if (emailError == null || string.IsNullOrEmpty(emailError.Text))
            {
                // login
                dialog.FindElement(By.Name("profile.firstName")).SendKeys(_firstName);
                dialog.FindElement(By.Name("profile.lastName")).SendKeys(_lastName);
                dialog.FindElement(By.Name("password")).SendKeys(_passsword);
                dialog.FindElement(By.Name("passwordRetype")).SendKeys(_passsword);
                dialog.FindElement(By.CssSelector("input.gigya-input-submit")).Click();

                // wait for expected error alert
                Thread.Sleep(5000);
                _driver.SwitchTo().Alert().Dismiss();
            }

            _driver.Navigate().GoToUrl(Config.Site1BaseURL);
            _driver.FindElement(By.CssSelector(".gigya-login"), 180).Click();

            dialog = _driver.FindElement(By.Id("gigya-login-screen"), 5);
            dialog.FindElement(By.Name("username")).SendKeys(_newEmail);
            dialog.FindElement(By.Name("password")).SendKeys(_passsword);
            dialog.FindElement(By.CssSelector("input.gigya-input-submit")).Click();

            // wait for expected error alert
            Thread.Sleep(5000);
            _driver.SwitchTo().Alert().Dismiss();
        }

        [TestMethod]
        public void IsLoggedIntoSecondSiteAsWell621()
        {
            if (!_loggedIn)
            {
                CanRegisterAndLoginToUmbraco621();
            }

            _driver.Navigate().GoToUrl(Config.Site2BaseURL);

            var logoutButton = _driver.FindElement(By.ClassName("gigya-logout"), 10);
            Assert.IsNotNull(logoutButton, "Logout button not found. User should be logged in.");
        }

        [TestMethod]
        public void CanLogoutAndIsLoggedOutOfSecondSiteAsWell621()
        {
            if (!_loggedIn)
            {
                CanRegisterAndLoginToUmbraco621();
            }

            // logout
            _driver.FindElement(By.CssSelector(".gigya-logout")).Click();

            // make sure register button is displayed which indicates that the user is logged out
            Assert.IsNotNull(_driver.FindElement(By.CssSelector(".gigya-register"), 5), "Register button not displayed. User may still be logged in or another error occurred.");

            // make sure user logged out of second site as well
            _driver.Navigate().GoToUrl(Config.Site2BaseURL);
            
            // make sure register button is displayed which indicates that the user is logged out
            Assert.IsNotNull(_driver.FindElement(By.CssSelector(".gigya-register"), 5), "Register button not displayed. User may still be logged in to second site or another error occurred.");
        }
        
        public void CanLoginToFrontEnd621()
        {
            _driver.Navigate().GoToUrl(Config.Site1BaseURL);

            // wait the usual 3 mins for sitefinity to start up....
            _driver.FindElement(By.CssSelector(".gigya-login"), 180).Click();

            var dialog = _driver.FindElement(By.Id("gigya-login-screen"), 5);
            dialog.FindElement(By.Name("username")).SendKeys(_newEmail);
            dialog.FindElement(By.Name("password")).SendKeys(_passsword);
            dialog.FindElement(By.CssSelector("input.gigya-input-submit")).Click();

            var logoutButton = _driver.FindElement(By.ClassName("gigya-logout"), 10);
            Assert.IsNotNull(logoutButton, "Logout button not found. User should be logged in.");

            _loggedIn = true;
        }

        [TestMethod]
        public void CanUpdateProfileUmbraco621()
        {
            CanRegisterAndLoginToUmbraco621();

            var application = new ConsoleApplicationBase();
            application.Start(application, new EventArgs());
            var context = ApplicationContext.Current;
            
            var memberService = context.Services.MemberService;
            var member = memberService.GetByEmail(_newEmail);

            Assert.IsNotNull(member, "Member not found in Umbraco after creation");

            // check fields are mapped
            Assert.IsTrue(member.HasProperty("firstName"), "Member not configured with firstName property. Check setup.");
            Assert.AreEqual(_firstName, member.GetValue("firstName"), "First name not mapped");
            Assert.IsTrue(member.HasProperty("lastName"), "Member not configured with lastName property. Check setup.");
            Assert.AreEqual(_lastName, member.GetValue("lastName"), "Last name not mapped");
        }

        [TestMethod]
        public void UmbracoFieldOverriddenOnLogin621()
        {
            CanRegisterAndLoginToUmbraco621();

            var application = new ConsoleApplicationBase();
            application.Start(application, new EventArgs());
            var context = ApplicationContext.Current;

            var memberService = context.Services.MemberService;
            var member = memberService.GetByEmail(_newEmail);
            Assert.IsNotNull(member, "Member not found in Umbraco after creation");

            // update member with some random crap
            member.SetValue("firstName", "laksjdfasjdflasjdklfajslkdf");
            member.SetValue("lastName", "lskdjfskldf");
            memberService.Save(member);

            var logoutButton = _driver.FindElement(By.ClassName("gigya-logout"), 10);
            Assert.IsNotNull(logoutButton, "Logout button not found. User should be logged in.");
            logoutButton.Click();

            CanLoginToFrontEnd621();

            member = memberService.GetByEmail(_newEmail);
            Assert.IsNotNull(member, "Member not found in Umbraco after creation");

            // check fields have been updated
            Assert.AreEqual(_firstName, member.GetValue("firstName"), "First name not mapped");
            Assert.AreEqual(_lastName, member.GetValue("lastName"), "Last name not mapped");
        }

        [TestMethod]
        public void IsSessionExpiredCorrectly621()
        {
            var application = new ConsoleApplicationBase();
            application.Start(application, new EventArgs());
            var context = ApplicationContext.Current;
            var db = context.DatabaseContext.Database;

            var sql = "SELECT * FROM gigya_settings";
            var settings = db.Fetch<GigyaUmbracoModuleSettings>(sql);
            var currentGlobalSettings = settings.Select(i => i.GlobalParameters).ToList();

            // update all to 10 seconds
            foreach (var setting in settings)
            {
                setting.GlobalParameters = "{ \"sessionExpiration\": 10 }";
                db.Save(setting);
            }

            var reset = false;

            try
            {
                // test session expiration works now that we know all sites are using 10 second timeout
                CanRegisterAndLoginToUmbraco621();

                IsLoggedIntoSecondSiteAsWell621();

                _driver.Navigate().GoToUrl(Config.Site1BaseURL);

                // wait for session to expire
                Thread.Sleep(11000);
                HasSessionExpired();

                // check logged out of site 2 as well
                _driver.Navigate().GoToUrl(Config.Site2BaseURL);
                HasSessionExpired();

                // login again
                CanLoginToFrontEnd621();

                ResetSettings(db, settings, currentGlobalSettings);
                reset = true;
            }
            catch
            {
                if (!reset)
                {
                    ResetSettings(db, settings, currentGlobalSettings);
                }
                throw;
            }
        }

        private void HasSessionExpired()
        {
            // reload page and should still be logged in
            _driver.Navigate().Refresh();
            var logoutButton = _driver.FindElement(By.ClassName("gigya-logout"), 10);
            Assert.IsNotNull(logoutButton, "Not logged in.");

            // wait for logout to complete
            Thread.Sleep(10000);

            _driver.Navigate().Refresh();

            logoutButton = _driver.FindElement(By.ClassName("gigya-logout"), 5);
            Assert.IsNull(logoutButton, "Still logged in after session timeout");
        }

        private static void ResetSettings(UmbracoDatabase db, System.Collections.Generic.List<GigyaUmbracoModuleSettings> settings, System.Collections.Generic.List<string> currentGlobalSettings)
        {
            // reset back to original values
            for (int i = 0; i < settings.Count; i++)
            {
                settings[i].GlobalParameters = currentGlobalSettings[i];
                db.Save(settings[i]);
            }
        }
    }
}
