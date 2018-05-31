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
using Umbraco.Core;
using Umbraco.Core.Services;
using Umbraco.Core.Persistence;
using Gigya.UnitTests.Umbraco;
using Gigya.Umbraco.Module.Data;
using Umbraco.Core.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using Gigya.Module.Core.Connector.Models;
using System.Xml;
using System.IO;

namespace Gigya.UnitTests.Selenium
{
    [TestClass]
    public abstract class RegisterLoginTests
    {
        protected IWebDriver _driver;

        protected string _passsword = "aa234567";
        protected const string _firstName = "Mr";
        protected const string _lastName = "Tester";

        protected const string _updatedFirstName = "Mickey";
        protected const string _updatedLastName = "Mouse";

        protected const string _umbracoFirstName = "firstName";
        protected const string _umbracoLastName = "lastName";
        protected const string _umbracoAge = "age";
        protected const string _gigyaAge = "profile.age";
        protected bool _loggedIn;

        protected string _newEmail;

        [TestInitialize]
        public void InitTest()
        {
            SetupTest();
        }

        protected virtual void SetupTest()
        {
            var options = new FirefoxOptions
            {
                BrowserExecutableLocation = "C:\\Program Files\\Mozilla Firefox\\Firefox.exe"
            };
            
            _driver = new FirefoxDriver(options);
            _driver.Manage().Window.Maximize();
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

        protected void CanRegisterAndLoginToCms()
        {
            _driver.Navigate().GoToUrl(Config.Site1BaseURL);

            // wait the usual 3 mins for sitefinity to start up....
            var button = _driver.FindElement(By.CssSelector(".gigya-register, .gigya-cms-register"), 10);
            if (button != null)
            {
                button.Click();
            }

            var dialog = _driver.FindElement(By.Id("gigya-register-screen"), 2);
            dialog.FindElement(By.Name("email")).SendKeys(_newEmail);

            dialog.FindElement(By.Name("profile.firstName")).SendKeys(_firstName);
            dialog.FindElement(By.Name("profile.lastName")).SendKeys(_lastName);
            dialog.FindElement(By.Name("password")).SendKeys(_passsword);
            dialog.FindElement(By.Name("passwordRetype")).SendKeys(_passsword);
            Thread.Sleep(1000);
            dialog.FindElement(By.CssSelector("input.gigya-input-submit")).Click();

            // wait up to 30 seconds for reload
            var logoutButton = _driver.FindElement(By.CssSelector(".gigya-logout, .gigya-cms-logout"), 10);
            Assert.IsNotNull(logoutButton, "Logout button not found. User should be logged in.");

            _loggedIn = true;
        }
        
        protected void CmsExceptionLogsInUser()
        {
            _newEmail = Gigya.Module.Core.Constants.Testing.EmailWhichThrowsException;

            _driver.Navigate().GoToUrl(Config.Site1BaseURL);

            // wait the usual 3 mins for sitefinity to start up....
            var button = _driver.FindElement(By.CssSelector(".gigya-register, .gigya-cms-register"), 10);
            if (button != null)
            {
                button.Click();
            }

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

            button = _driver.FindElement(By.CssSelector(".gigya-login, .gigya-cms-login"), 10);
            if (button != null)
            {
                button.Click();
            }

            dialog = _driver.FindElement(By.Id("gigya-login-screen"), 5);
            dialog.FindElement(By.Name("username")).SendKeys(_newEmail);
            dialog.FindElement(By.Name("password")).SendKeys(_passsword);
            dialog.FindElement(By.CssSelector("input.gigya-input-submit")).Click();

            // wait for expected error alert
            Thread.Sleep(5000);
            _driver.SwitchTo().Alert().Dismiss();
        }
        
        protected void IsLoggedIntoSecondSiteAsWell()
        {
            if (!_loggedIn)
            {
                CanRegisterAndLoginToCms();
            }

            _driver.Navigate().GoToUrl(Config.Site2BaseURL);

            Thread.Sleep(10000);

            _driver.Navigate().Refresh();

            var logoutButton = _driver.FindElement(By.CssSelector(".gigya-logout, .gigya-cms-logout"), 10);
            Assert.IsNotNull(logoutButton, "Logout button not found. User should be logged in.");
        }
        
        protected void CanLogoutAndIsLoggedOutOfSecondSiteAsWell()
        {
            if (!_loggedIn)
            {
                CanRegisterAndLoginToCms();
            }

            // logout
            _driver.FindElement(By.CssSelector(".gigya-logout, .gigya-cms-logout")).Click();

            Thread.Sleep(5000);

            // make sure register button is displayed which indicates that the user is logged out
            Assert.IsNotNull(_driver.FindElement(By.CssSelector(".gigya-register, #gigya-register-screen, .gigya-cms-register"), 10), "Register button not displayed. User may still be logged in or another error occurred.");

            // make sure user logged out of second site as well
            _driver.Navigate().GoToUrl(Config.Site2BaseURL);
            
            // make sure register button is displayed which indicates that the user is logged out
            Assert.IsNotNull(_driver.FindElement(By.CssSelector(".gigya-register, #gigya-register-screen, .gigya-cms-register"), 10), "Register button not displayed. User may still be logged in to second site or another error occurred.");
        }

        protected void CanLoginToFrontEnd()
        {
            CanLoginToFrontEnd(Config.Site1BaseURL);
        }

        protected void CanLoginToFrontEnd(string url)
        {
            _driver.Navigate().GoToUrl(url);

            // wait the usual 3 mins for sitefinity to start up....

            // .gigya-cms-login 

            var loginButton = _driver.FindElement(By.CssSelector(".gigya-login, .gigya-cms-login"), 10);
            if (loginButton != null)
            {
                loginButton.Click();
            }

            var dialog = _driver.FindElement(By.Id("gigya-login-screen"), 5);
            dialog.FindElement(By.Name("username")).SendKeys(_newEmail);
            dialog.FindElement(By.Name("password")).SendKeys(_passsword);
            dialog.FindElement(By.CssSelector("input.gigya-input-submit")).Click();

            var logoutButton = _driver.FindElement(By.CssSelector(".gigya-logout, .gigya-cms-logout"), 10);
            Assert.IsNotNull(logoutButton, "Logout button not found. User should be logged in.");

            _loggedIn = true;
        }

        protected void HasSessionExpired()
        {
            // reload page and should still be logged in
            _driver.Navigate().Refresh();
            var logoutButton = _driver.FindElement(By.CssSelector(".gigya-logout, .gigya-cms-logout"), 10);
            Assert.IsNotNull(logoutButton, "Not logged in.");

            // wait for logout to complete
            Thread.Sleep(10000);

            _driver.Navigate().Refresh();

            logoutButton = _driver.FindElement(By.CssSelector(".gigya-logout, .gigya-cms-logout"), 5);
            Assert.IsNull(logoutButton, "Still logged in after session timeout");
        }
    }
}
