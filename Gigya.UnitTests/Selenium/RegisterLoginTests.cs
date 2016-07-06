using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Text;
using OpenQA.Selenium.Firefox;
using System.Configuration;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Remote;

namespace Gigya.UnitTests.Selenium
{
    [TestClass]
    public class RegisterLoginTests
    {
        private IWebDriver _driver;
        private IWebDriver _sitefinityAdminDriver;

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
        public void CanRegisterAndLoginToSitefinity()
        {
            _driver.Navigate().GoToUrl(Config.Site1BaseURL);

            // wait the usual 3 mins for sitefinity to start up....
            _driver.FindElement(By.Id("gigya-register"), 180).Click();
            
            var dialog = _driver.FindElement(By.Id("gigya-register-screen"), 2);
            dialog.FindElement(By.Name("email")).SendKeys(_newEmail);

            dialog.FindElement(By.Name("profile.firstName")).SendKeys(_firstName);
            dialog.FindElement(By.Name("profile.lastName")).SendKeys(_lastName);
            dialog.FindElement(By.Name("password")).SendKeys(_passsword);
            dialog.FindElement(By.Name("passwordRetype")).SendKeys(_passsword);
            dialog.FindElement(By.CssSelector("input.gigya-input-submit")).Click();

            // wait up to 30 seconds for reload
            var loggedInGreeting = _driver.FindElement(By.ClassName("gigya-logged-greeting"), 30);
            Assert.IsNotNull(loggedInGreeting, "Logged in greeting not found. User should be logged in.");

            Assert.IsTrue(loggedInGreeting.Text.Contains(_newEmail), "Logged in greeting found but doesn't contain new user's email.");

            _loggedIn = true;
        }

        [TestMethod]
        public void SitefinityExceptionLogsInUser()
        {
            _newEmail = Gigya.Module.Constants.Testing.EmailWhichThrowsException;

            _driver.Navigate().GoToUrl(Config.Site1BaseURL);

            // wait the usual 3 mins for sitefinity to start up....
            _driver.FindElement(By.Id("gigya-register"), 180).Click();

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
            _driver.FindElement(By.Id("gigya-login"), 180).Click();

            dialog = _driver.FindElement(By.Id("gigya-login-screen"), 5);
            dialog.FindElement(By.Name("username")).SendKeys(_newEmail);
            dialog.FindElement(By.Name("password")).SendKeys(_passsword);
            dialog.FindElement(By.CssSelector("input.gigya-input-submit")).Click();

            // wait for expected error alert
            Thread.Sleep(5000);
            _driver.SwitchTo().Alert().Dismiss();
        }

        [TestMethod]
        public void IsLoggedIntoSecondSiteAsWell()
        {
            if (!_loggedIn)
            {
                CanRegisterAndLoginToSitefinity();
            }

            _driver.Navigate().GoToUrl(Config.Site2BaseURL);

            var loggedInGreeting = _driver.FindElement(By.ClassName("gigya-logged-greeting"), 30);
            Assert.IsNotNull(loggedInGreeting, "Logged in greeting not found. User should be logged in to second site.");

            Assert.IsTrue(loggedInGreeting.Text.Contains(_newEmail), "Logged in greeting found but doesn't contain new user's email.");
        }

        [TestMethod]
        public void CanLogoutAndIsLoggedOutOfSecondSiteAsWell()
        {
            if (!_loggedIn)
            {
                CanRegisterAndLoginToSitefinity();
            }

            // logout
            _driver.FindElement(By.Id("gigya-logout")).Click();

            // make sure register button is displayed which indicates that the user is logged out
            Assert.IsNotNull(_driver.FindElement(By.Id("gigya-register"), 5), "Register button not displayed. User may still be logged in or another error occurred.");

            // make sure user logged out of second site as well
            _driver.Navigate().GoToUrl(Config.Site2BaseURL);
            
            // make sure register button is displayed which indicates that the user is logged out
            Assert.IsNotNull(_driver.FindElement(By.Id("gigya-register"), 5), "Register button not displayed. User may still be logged in to second site or another error occurred.");
        }
        
        public void CanLoginToFrontEnd()
        {
            _driver.Navigate().GoToUrl(Config.Site1BaseURL);

            // wait the usual 3 mins for sitefinity to start up....
            _driver.FindElement(By.Id("gigya-login"), 180).Click();

            var dialog = _driver.FindElement(By.Id("gigya-login-screen"), 5);
            dialog.FindElement(By.Name("username")).SendKeys(_newEmail);
            dialog.FindElement(By.Name("password")).SendKeys(_passsword);
            dialog.FindElement(By.CssSelector("input.gigya-input-submit")).Click();

            // wait up to 30 seconds for reload
            var loggedInGreeting = _driver.FindElement(By.ClassName("gigya-logged-greeting"), 30);
            Assert.IsNotNull(loggedInGreeting, "Logged in greeting not found. User should be logged in.");

            Assert.IsTrue(loggedInGreeting.Text.Contains(_newEmail), "Logged in greeting found but doesn't contain new user's email.");

            _loggedIn = true;
        }

        [TestMethod]
        public void CanUpdateProfile()
        {
            if (!_loggedIn)
            {
                CanRegisterAndLoginToSitefinity();
            }

            CanLogoutAndIsLoggedOutOfSecondSiteAsWell();

            CanLoginToFrontEnd();

            _driver.FindElement(By.Id("gigya-edit-profile")).Click();

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

        private void LoginToSitefinityAdmin()
        {
            _sitefinityAdminDriver = new FirefoxDriver();

            SitefinityUtils.LoginToSitefinity(_sitefinityAdminDriver);
        }
    }
}
