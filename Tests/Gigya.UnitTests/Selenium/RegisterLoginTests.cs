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
    public class RegisterLoginTests
    {
        private IWebDriver _driver;
        private IWebDriver _sitefinityAdminDriver;

        private const string _passsword = "aa234567";
        private const string _firstName = "Mr";
        private const string _lastName = "Tester";
        
        private const string _updatedFirstName = "Mickey";
        private const string _updatedLastName = "Mouse";

		private const string _umbracoFirstName = "firstName";
        private const string _umbracoLastName = "lastName";
        private const string _umbracoAge = "age";
        private const string _gigyaAge = "profile.age";
        private bool _loggedIn;

        private string _newEmail;

        [TestInitialize]
        public void SetupTest()
        {
            _driver = new FirefoxDriver();
            _driver.Manage().Window.Maximize();
            _newEmail = string.Concat(Guid.NewGuid(), "@purestone.co.uk");

            var installation = new UmbracoInstallationTests(_driver);
            installation.SetupNewUmbracoSiteIfRequired();
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
        public void CanRegisterAndLoginToCms()
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
        public void IsLoggedIntoSecondSiteAsWell()
        {
            if (!_loggedIn)
            {
                CanRegisterAndLoginToCms();
            }

            _driver.Navigate().GoToUrl(Config.Site2BaseURL);

            var logoutButton = _driver.FindElement(By.ClassName("gigya-logout"), 10);
            Assert.IsNotNull(logoutButton, "Logout button not found. User should be logged in.");
        }

        [TestMethod]
        public void CanLogoutAndIsLoggedOutOfSecondSiteAsWell()
        {
            if (!_loggedIn)
            {
                CanRegisterAndLoginToCms();
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
        
        public void CanLoginToFrontEnd()
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

        private void CreateMemberTypesIfRequired(ApplicationContext context)
        {
            var memberType = context.Services.MemberTypeService.Get("member");
            var textBoxDefinition = new DataTypeDefinition( -1, "text");
            const int textStringId = -88;
            const int numericId = -51;
            var numericDefinition = new DataTypeDefinition(-1, "numeric");
            
            var updated = false;

            if (!memberType.PropertyTypeExists("firstName"))
            {
                var propertyType = new PropertyType(textBoxDefinition) { Alias = "firstName", DataTypeDefinitionId = textStringId, Name = "First Name", Description = "", Mandatory = false, SortOrder = 1 };

                memberType.AddPropertyType(propertyType, "Membership");
                updated = true;
            }
            if (!memberType.PropertyTypeExists("lastName"))
            {
                memberType.AddPropertyType(new PropertyType(textBoxDefinition) { Alias = "lastName", DataTypeDefinitionId = textStringId, Name = "Last Name", Description = "", Mandatory = false, SortOrder = 1 }, "Membership");
                updated = true;
            }
            if (!memberType.PropertyTypeExists("age"))
            {
                memberType.AddPropertyType(new PropertyType(numericDefinition) { Alias = "age", DataTypeDefinitionId = numericId, Name = "Age", Description = "", Mandatory = false, SortOrder = 1 }, "Membership");
                updated = true;
            }

            if (updated)
            {
                context.Services.MemberTypeService.Save(memberType);
            }
        }

        [TestMethod]
        public void CanUpdateProfileUmbraco()
        {
            var application = new ConsoleApplicationBase();
            application.Start(application, new EventArgs());
            var context = ApplicationContext.Current;

            CreateMemberTypesIfRequired(context);
			CreateFieldMappings(context.DatabaseContext.Database);
            CanRegisterAndLoginToCms();
			
			_driver.FindElement(By.CssSelector(".gigya-edit-profile")).Click();

            // wait for form
            var form = _driver.FindElement(By.Id("gigya-profile-form"), 5);
            form.FindElement(By.Name("profile.firstName")).ClearAndSendKeys(_updatedFirstName);
            form.FindElement(By.Name("profile.lastName")).ClearAndSendKeys(_updatedLastName);

            // set DOB
            var age = 50;
            var dob = DateTime.Now.AddYears(age * -1);

            form.FindElement(By.Name("profile.birthDay")).SendKeys(dob.Day.ToString());
            form.FindElement(By.Name("profile.birthMonth")).SendKeys(dob.Month.ToString());
            form.FindElement(By.Name("profile.birthYear")).SendKeys(dob.Year.ToString());

            // save profile
            form.FindElement(By.CssSelector("input.gigya-input-submit")).Click();

            // wait for profile update
            Thread.Sleep(5000);

            var memberService = context.Services.MemberService;
            var member = memberService.GetByEmail(_newEmail);

            Assert.IsNotNull(member, "Member not found in Umbraco after creation");

            // check fields are mapped
            Assert.IsTrue(member.HasProperty(_umbracoFirstName), "Member not configured with firstName property. Check setup.");
            Assert.AreEqual(_updatedFirstName, member.GetValue(_umbracoFirstName), "First name not mapped");

            Assert.IsTrue(member.HasProperty(_umbracoLastName), "Member not configured with lastName property. Check setup.");
            Assert.AreEqual(_updatedLastName, member.GetValue(_umbracoLastName), "Last name not mapped");

            Assert.IsTrue(member.HasProperty(_umbracoAge), "Member not configured with age property. Check setup.");
            Assert.AreEqual(age, Convert.ToInt32(member.GetValue(_umbracoAge)), "Age not mapped");
        }

		private void CreateFieldMappings(UmbracoDatabase db)
        {
            var sql = "SELECT * FROM gigya_settings";
            var settings = db.Fetch<GigyaUmbracoModuleSettings>(sql);
            var currentGlobalSettings = settings.Select(i => i.GlobalParameters).ToList();
            
            // create field mappings if don't already exist
            foreach (var setting in settings)
            {
                var mappings = !string.IsNullOrEmpty(setting.MappingFields) ? JsonConvert.DeserializeObject<List<MappingField>>(setting.MappingFields) : new List<MappingField>();
                CreateFieldMapping(ref mappings, _umbracoFirstName, Gigya.Umbraco.Module.Constants.GigyaFields.FirstName);
                CreateFieldMapping(ref mappings, _umbracoLastName, Gigya.Umbraco.Module.Constants.GigyaFields.LastName);
                CreateFieldMapping(ref mappings, _umbracoAge, _gigyaAge);

                setting.MappingFields = JsonConvert.SerializeObject(mappings);
                db.Save(setting);
            }
        }

        private static void CreateFieldMapping(ref List<MappingField> mappings, string cmsFieldName, string gigyaFieldName)
        {
            if (!mappings.Any(i => i.CmsFieldName == cmsFieldName))
            {
                mappings.Add(new MappingField
                {
                    CmsFieldName = cmsFieldName,
                    GigyaFieldName = gigyaFieldName
                });
            }
        }

        [TestMethod]
        public void UmbracoFieldOverriddenOnLogin()
        {
            var application = new ConsoleApplicationBase();
            application.Start(application, new EventArgs());
            var context = ApplicationContext.Current;

			CreateFieldMappings(context.DatabaseContext.Database);
            CreateMemberTypesIfRequired(context);
            CanRegisterAndLoginToCms();

            var memberService = context.Services.MemberService;
            var member = memberService.GetByEmail(_newEmail);
            Assert.IsNotNull(member, "Member not found in Umbraco after creation");

            // update member with some random crap so we can check it is updated correctly
            member.SetValue(_umbracoFirstName, "laksjdfasjdflasjdklfajslkdf");
            member.SetValue(_umbracoLastName, "lskdjfskldf");
            memberService.Save(member);

            var logoutButton = _driver.FindElement(By.ClassName("gigya-logout"), 10);
            Assert.IsNotNull(logoutButton, "Logout button not found. User should be logged in.");
            logoutButton.Click();

            CanLoginToFrontEnd();

            member = memberService.GetByEmail(_newEmail);
            Assert.IsNotNull(member, "Member not found in Umbraco after creation");

            // check fields have been updated
            Assert.AreEqual(_firstName, member.GetValue("firstName"), "First name not mapped");
            Assert.AreEqual(_lastName, member.GetValue("lastName"), "Last name not mapped");
        }

        [TestMethod]
        public void IsSessionExpiredCorrectlyUmbraco()
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

        [TestMethod]
        public void IsUserLoggedInAfterUmbracoSessionExpired()
        {
            XmlDocument webConfigDoc = new XmlDocument();
            webConfigDoc.Load(Path.Combine(Config.UmbracoRootPath, "web.config"));

            // update session timeout in web.config
            var sessionElem = webConfigDoc.SelectSingleNode("/configuration/system.web/sessionState");
            if (sessionElem == null)
            {
                sessionElem = webConfigDoc.CreateElement("sessionState");
                webConfigDoc.SelectSingleNode("/configuration/system.web").AppendChild(sessionElem);
            }

            var timeoutAttribute = sessionElem.Attributes["timeout"];
            if (timeoutAttribute == null)
            {
                timeoutAttribute = webConfigDoc.CreateAttribute("timeout");
                sessionElem.Attributes.Append(timeoutAttribute);
            }

            timeoutAttribute.Value = "1";

            // update forms timeout in web.config
            var formsElem = webConfigDoc.SelectSingleNode("/configuration/system.web/authentication/forms");
            var formsTimeoutAttribute = formsElem.Attributes["timeout"];
            if (formsTimeoutAttribute == null)
            {
                formsTimeoutAttribute = webConfigDoc.CreateAttribute("timeout");
                formsElem.Attributes.Append(formsTimeoutAttribute);
            }

            formsTimeoutAttribute.Value = "1";
            timeoutAttribute.Value = "1";

            webConfigDoc.Save(Path.Combine(Config.UmbracoRootPath, "web.config"));

            var application = new ConsoleApplicationBase();
            application.Start(application, new EventArgs());
            var context = ApplicationContext.Current;
            var db = context.DatabaseContext.Database;

            var sql = "SELECT * FROM gigya_settings";
            var settings = db.Fetch<GigyaUmbracoModuleSettings>(sql);
            var currentGlobalSettings = settings.Select(i => i.GlobalParameters).ToList();

            // update all to 80 seconds
            foreach (var setting in settings)
            {
                setting.GlobalParameters = "{ \"sessionExpiration\": 500 }";
                db.Save(setting);
            }

            var reset = false;

            try
            {
                // test session expiration works now that we know all sites are using 10 second timeout
                CanRegisterAndLoginToCms();

                IsLoggedIntoSecondSiteAsWell();

                _driver.Navigate().GoToUrl(Config.Site1BaseURL);

                // wait for umbraco session to expire
                Thread.Sleep(70000);

                // refresh page and should now be logged out as umbraco session is set to 1 min
                _driver.Navigate().Refresh();

                // user should now be logged out
                var loginButton = _driver.FindElement(By.ClassName("gigya-login"), 10);
                Assert.IsNotNull(loginButton, "Login button not found. User should be logged out.");

                Thread.Sleep(5000);

                // refresh page and should now be logged in as gigya session timeout is more than umbraco
                _driver.Navigate().Refresh();

                var logoutButton = _driver.FindElement(By.ClassName("gigya-logout"), 10);
                Assert.IsNotNull(logoutButton, "Logout button not found. User should be logged in.");
                
                ResetSettings(db, settings, currentGlobalSettings);

                // reset
                formsTimeoutAttribute.Value = "30";
                timeoutAttribute.Value = "30";
                webConfigDoc.Save(Path.Combine(Config.UmbracoRootPath, "web.config"));

                reset = true;
            }
            catch
            {
                if (!reset)
                {
                    ResetSettings(db, settings, currentGlobalSettings);

                    // reset
                    formsTimeoutAttribute.Value = "30";
                    timeoutAttribute.Value = "30";
                    webConfigDoc.Save(Path.Combine(Config.UmbracoRootPath, "web.config"));
                }
                throw;
            }
        }

        [TestMethod]
        public void RemoveUmbracoCookieAndUserIsLoggedInAfterRefresh()
        {
            CanRegisterAndLoginToCms();

            _driver.Manage().Cookies.DeleteCookieNamed("yourAuthCookie");

            _driver.Navigate().Refresh();

            // user should be logged out as cookie deleted
            var loginButton = _driver.FindElement(By.ClassName("gigya-login"), 10);
            Assert.IsNotNull(loginButton, "Login button not found. User should be logged out.");

            Thread.Sleep(5000);

            _driver.Navigate().Refresh();
            var logoutButton = _driver.FindElement(By.ClassName("gigya-logout"), 10);
            Assert.IsNotNull(logoutButton, "Logout button not found. User should be logged in.");
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

        private static void ResetSettings(UmbracoDatabase db, List<GigyaUmbracoModuleSettings> settings, List<string> currentGlobalSettings)
        {
            // reset back to original values
            for (int i = 0; i < settings.Count; i++)
            {
                settings[i].GlobalParameters = currentGlobalSettings[i];
                db.Save(settings[i]);
            }
        }

        [TestMethod]
        public void CanUpdateProfileSitefinity()
        {
            if (!_loggedIn)
            {
                CanRegisterAndLoginToCms();
            }

            CanLogoutAndIsLoggedOutOfSecondSiteAsWell();

            CanLoginToFrontEnd();

            _driver.FindElement(By.CssSelector(".gigya-edit-profile")).Click();

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
