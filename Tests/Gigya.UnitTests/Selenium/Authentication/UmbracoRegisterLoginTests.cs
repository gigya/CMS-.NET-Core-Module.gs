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

namespace Gigya.UnitTests.Selenium.Authentication
{
    [TestClass]
    public class UmbracoRegisterLoginTests : RegisterLoginTests
    {
        protected override void SetupTest()
        {
            base.SetupTest();

            var installation = new UmbracoInstallationTests(_driver);
            installation.Umbraco_SetupNewUmbracoSiteIfRequired();
        }

        [TestMethod]
        public void Umbraco_CanRegisterAndLoginToCms()
        {
            base.CanRegisterAndLoginToCms();
        }

        [TestMethod]
        public void Umbraco_IsLoggedIntoSecondSiteAsWell()
        {
            base.IsLoggedIntoSecondSiteAsWell();
        }

        [TestMethod]
        public void Umbraco_CmsExceptionLogsInUser()
        {
            base.CmsExceptionLogsInUser();
        }

        [TestMethod]
        public void Umbraco_CanLogoutAndIsLoggedOutOfSecondSiteAsWell()
        {
            base.CanLogoutAndIsLoggedOutOfSecondSiteAsWell();
        }

        private void CreateMemberTypesIfRequired(ApplicationContext context)
        {
            var memberType = context.Services.MemberTypeService.Get("member");
            var textBoxDefinition = new DataTypeDefinition(-1, "text");
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
        public void Umbraco_CanUpdateProfile()
        {
            using (var application = new ConsoleApplicationBase())
            {
                application.Start(application, new EventArgs());
                var context = ApplicationContext.Current;

                //CreateMemberTypesIfRequired(context);
                //CreateFieldMappings(context.DatabaseContext.Database);

                CanRegisterAndLoginToCms();

                var button = _driver.FindElement(By.CssSelector(".gigya-edit-profile"), 10);
                if (button != null)
                {
                    button.Click();
                }

                // wait for form
                var form = _driver.FindElement(By.Id("gigya-profile-form"), 5);
                form.FindElement(By.Name("profile.firstName")).ClearAndSendKeys(_updatedFirstName);
                form.FindElement(By.Name("profile.lastName")).ClearAndSendKeys(_updatedLastName);
                form.FindElement(By.Name("profile.email")).ClearAndSendKeys(_newEmail);

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
        public void Umbraco_FieldOverriddenOnLogin()
        {
            using (var application = new ConsoleApplicationBase())
            {
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
        }

        [TestMethod]
        public void Umbraco_IsSessionExpiredCorrectly()
        {
            using (var application = new ConsoleApplicationBase())
            {
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
        }

        [TestMethod]
        public void Umbraco_IsUserLoggedInAfterSessionExpired()
        {
            SetFormsTimeout(1);

            using (var application = new ConsoleApplicationBase())
            {
                application.Start(application, new EventArgs());
                var context = ApplicationContext.Current;
                var db = context.DatabaseContext.Database;

                var sql = "SELECT * FROM gigya_settings";
                var settings = db.Fetch<GigyaUmbracoModuleSettings>(sql);
                var currentGlobalSettings = settings.Select(i => i.GlobalParameters).ToList();

                // update all to 500 seconds
                foreach (var setting in settings)
                {
                    setting.GlobalParameters = "{ \"sessionExpiration\": 500 }";
                    db.Save(setting);
                }

                var reset = false;

                try
                {
                    // test session expiration works now that we know all sites are using 500 second timeout
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
                    SetFormsTimeout(30);

                    reset = true;
                }
                catch
                {
                    if (!reset)
                    {
                        ResetSettings(db, settings, currentGlobalSettings);

                        // reset
                        SetFormsTimeout(30);
                    }
                    throw;
                }
            }

            Thread.Sleep(5000);
        }

        private static void SetFormsTimeout(int timeout)
        {
            XmlDocument webConfigDoc;
            XmlAttribute timeoutAttribute, formsTimeoutAttribute;
            webConfigDoc = new XmlDocument();
            webConfigDoc.Load(Path.Combine(Config.UmbracoRootPath, "web.config"));

            // update session timeout in web.config
            var sessionElem = webConfigDoc.SelectSingleNode("/configuration/system.web/sessionState");
            if (sessionElem == null)
            {
                sessionElem = webConfigDoc.CreateElement("sessionState");
                webConfigDoc.SelectSingleNode("/configuration/system.web").AppendChild(sessionElem);
            }

            timeoutAttribute = sessionElem.Attributes["timeout"];
            if (timeoutAttribute == null)
            {
                timeoutAttribute = webConfigDoc.CreateAttribute("timeout");
                sessionElem.Attributes.Append(timeoutAttribute);
            }

            timeoutAttribute.Value = timeout.ToString();

            // update forms timeout in web.config
            var formsElem = webConfigDoc.SelectSingleNode("/configuration/system.web/authentication/forms");
            formsTimeoutAttribute = formsElem.Attributes["timeout"];
            if (formsTimeoutAttribute == null)
            {
                formsTimeoutAttribute = webConfigDoc.CreateAttribute("timeout");
                formsElem.Attributes.Append(formsTimeoutAttribute);
            }

            formsTimeoutAttribute.Value = timeout.ToString();
            timeoutAttribute.Value = timeout.ToString();

            webConfigDoc.Save(Path.Combine(Config.UmbracoRootPath, "web.config"));
        }

        [TestMethod]
        public void Umbraco_RemoveCookieAndUserIsLoggedInAfterRefresh()
        {
            CanRegisterAndLoginToCms();

            _driver.Manage().Cookies.DeleteCookieNamed("yourAuthCookie");

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
        public void Umbraco_IsLoggedIntoGigyaAfterTimeout_UmbracoSessionProvider()
        {
            SetFormsTimeout(1);

            // configure web.config timeout to 
            using (var application = new ConsoleApplicationBase())
            {
                application.Start(application, new EventArgs());
                var context = ApplicationContext.Current;
                var db = context.DatabaseContext.Database;
                
                var sql = "UPDATE gigya_settings SET SessionProvider = 1, GlobalParameters = '{ \"sessionExpiration\": 10 }'";
                var settings = db.ExecuteScalar<GigyaUmbracoModuleSettings>(sql);
                
                // login to CMS
                CanRegisterAndLoginToCms();

                // wait 11 secs
                Thread.Sleep(11000);

                _driver.Navigate().Refresh();

                //Thread.Sleep(5000);

                // check that user is logged into Gigya
                var cookies = _driver.Manage().Cookies.AllCookies;
                if (!cookies.Any(i => i.Name.StartsWith("glt_3_qkAT5OcGyvYpkjc_VF6-OfoeTKGk4T_jVwjFF9f5TQzoAg-mH8SBsjQi1srdsOm6")))
                {
                    Assert.Fail("Failed to find Gigya cookie starting with glt");
                }

                Thread.Sleep(5000);
            }
        }

        [TestMethod]
        public void Umbraco_IsLoggedOutOfGigyaAfterTimeout_UmbracoSessionProvider()
        {
            SetFormsTimeout(1);

            // configure web.config timeout to 
            using (var application = new ConsoleApplicationBase())
            {
                application.Start(application, new EventArgs());
                var context = ApplicationContext.Current;
                var db = context.DatabaseContext.Database;

                var sql = "UPDATE gigya_settings SET SessionProvider = 1, GlobalParameters = '{ \"sessionExpiration\": 100 }'";
                var settings = db.ExecuteScalar<GigyaUmbracoModuleSettings>(sql);

                // login to CMS
                CanRegisterAndLoginToCms();

                // wait 70 secs
                Thread.Sleep(70000);

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
}
