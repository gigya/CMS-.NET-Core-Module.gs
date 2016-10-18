using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.IO;
using System.Threading;
using OpenQA.Selenium.Interactions;
using System.Xml;
using System.Xml.Linq;
using Gigya.UnitTests.Umbraco;
using Umbraco.Core;
using umbraco.BusinessLogic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Gigya.UnitTests.Selenium
{
    [TestClass]
    public class UmbracoInstallationTests
    {
        private IWebDriver _driver;

        public UmbracoInstallationTests()
        {

        }

        public UmbracoInstallationTests(IWebDriver driver)
        {
            _driver = driver;
        }

        [TestInitialize]
        public void SetupTest()
        {
            if (_driver == null)
            {
                _driver = new FirefoxDriver();
                _driver.Manage().Window.Maximize();
            }
            Umbraco_SetupNewUmbracoSiteIfRequired();
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
        public void Umbraco_SetupNewUmbracoSiteIfRequired()
        {
            _driver.Navigate().GoToUrl(Config.Site1BaseURL + "umbraco");

            LoginToUmbraco(Config.AdminEmail, Config.AdminPassword);

            if (!CreateUser())
            {
                return;
            }

            // close upgrade notification if visible
            var alert = _driver.FindElement(By.CssSelector(".alert-info .close"), 5);
            if (alert != null)
            {
                alert.Click();
            }

            InstallPackage();
            EnableGigyaForAdmin();
            AddGigyaSettingsToMaster();
            EnableMacrosOnHomepage();
            AddMacrosToHomepage();
            CopyHomepage();
            Utils.AddEncryptionKey(Config.UmbracoRootPath);
            EnterAllGigyaSettings();
        }

        [TestMethod]
        public void Umbraco_IsApplicationSecretHiddenForNonAdmin()
        {
            // create non admin user if required
            if (!LoginToUmbraco(Config.NonAdminUsername, Config.NonAdminPassword, false))
            {
                Assert.Fail(string.Format("Could not login to Umbraco with a non admin user with username: {0} and password: {1}. Please create a new non admin user with these credentials and enable the Gigya section.", Config.NonAdminUsername, Config.NonAdminPassword));
            }

            // navigate to gigya settings and shouldn't see application secret field
            _driver.Navigate().GoToUrl(Config.Site1BaseURL + "umbraco#/gigya/gigyaTree/edit/-1");
            _driver.Navigate().Refresh();

            var apiKeyField = _driver.FindElement(By.Id("api-key"), 10);
            Assert.IsNotNull(apiKeyField, "Application key field should be visible.");

            var applicationSecret = _driver.FindElement(By.Id("application-secret"), 2);
            Assert.IsNull(applicationSecret, "Application secret should not be visible.");
        }

        private void AddGigyaSettingsToMaster()
        {
            var gigyaSettings = "@Html.Action(\"Index\", \"GigyaSettings\")";
            var filePath = Path.Combine(Config.UmbracoRootPath, "Views", "Master.cshtml");

            // reading as string as HtmlAgilityPack changes some of the markup e.g. <img> instead of <img /> which annoys me.
            var master = File.ReadAllText(filePath);

            // add gigya settings
            if (!master.Contains(gigyaSettings))
            {
                master = master.Replace("</head>", gigyaSettings + "\n</head>");
                File.WriteAllText(filePath, master);
            }
        }

        private void EnableMacrosOnHomepage()
        {
            Thread.Sleep(5000);
            _driver.Navigate().GoToUrl(Config.Site1BaseURL + "umbraco#/developer/datatype/edit/1051");
            _driver.SwitchTo().DefaultContent();

            _driver.FindElement(By.CssSelector(".uSky-templates-rows .preview-row .preview-col"), 10).Click();
            _driver.FindElement(By.CssSelector(".usky-grid-configuration .uSky-templates-column"), 10).Click();
            _driver.FindElement(By.XPath("//localize[@key='grid_allowAllEditors']"), 5).Click();
            _driver.FindElement(By.CssSelector(".umb-panel-footer .btn-primary")).Click();
            _driver.FindElement(By.ClassName("btn-success"), 5).Click();
            Thread.Sleep(5000);
        }

        private void AddMacroToHomepage(string macroName)
        {
            Thread.Sleep(2000);
            _driver.FindElement(By.CssSelector(".cell-tools-add .iconBox"), 10).Click();
            Thread.Sleep(500);

            var macro = _driver.FindElement(By.PartialLinkText("Macro"), 5);

            if (macro == null)
            {
                // try again...
                _driver.FindElement(By.CssSelector(".cell-tools-add .iconBox"), 10).Click();
                Thread.Sleep(500);
                macro = _driver.FindElement(By.PartialLinkText("Macro"), 5);
            }

            macro.Click();
            var modal = _driver.FindElement(By.Name("insertMacroForm"), 5);
            Thread.Sleep(2000);

            var select = modal.FindElement(By.TagName("select"));
            select.SendKeys(macroName);
            select.SendKeys(Keys.Tab);

            Thread.Sleep(2000);
            modal.FindElement(By.ClassName("btn-primary")).Click();
            Thread.Sleep(5000);
        }

        private void AddMacrosToHomepage()
        {
            _driver.Navigate().GoToUrl(Config.Site1BaseURL + "umbraco#/content");

            Thread.Sleep(5000);

            var tree = _driver.FindElement(By.Id("tree"), 5);
            tree.FindElement(By.LinkText("Home")).Click();

            Thread.Sleep(5000);

            AddMacroToHomepage("Gigya Login Button");
            AddMacroToHomepage("Gigya Register Button");
            AddMacroToHomepage("Gigya Logout Button");
            AddMacroToHomepage("Gigya Edit Profile");

            // save page
            _driver.FindElement(By.CssSelector(".umb-bottom-bar .btn-success"), 5).Click();

            var successAlert = _driver.FindElement(By.ClassName("alert-success"), 10);
            Assert.IsNotNull(successAlert, "Failed to save homepage.");
        }

        private void CopyHomepage()
        {
            var homepage = _driver.FindElement(By.CssSelector("#tree ul > li > ul > li"), 5);
            Actions builder = new Actions(_driver);
            builder.ContextClick(homepage).Perform();

            var menu = _driver.FindElement(By.Id("contextMenu"), 5);
            Thread.Sleep(1000);
            menu.FindElement(By.PartialLinkText("Copy")).Click();
            Thread.Sleep(1000);
            _driver.FindElement(By.CssSelector("#dialog .root .root-link"), 5).Click();
            Thread.Sleep(1000);

            var dialog = _driver.FindElement(By.Id("dialog"), 5);
            dialog.FindElement(By.CssSelector(".umb-dialog-footer .btn-primary")).Click();

            // wait for copy to finish
            var alert = _driver.FindElement(By.CssSelector("#dialog .alert-success .btn-primary"), 20);
            Assert.IsNotNull(alert, "Copy failed.");

            alert.Click();
            Thread.Sleep(1000);

            var homepages = _driver.FindElements(By.CssSelector("#tree ul > li > ul > li"));
            foreach (var node in homepages)
            {
                if (node.FindElement(By.CssSelector("div")).GetAttribute("className").Contains("not-published"))
                {
                    builder = new Actions(_driver);
                    builder.ContextClick(node).Perform();
                    Thread.Sleep(1000);

                    menu.FindElement(By.PartialLinkText("Publish")).Click();
                    Thread.Sleep(2000);

                    var iframe = _driver.FindElement(By.CssSelector("#dialog iframe"), 10);
                    _driver.SwitchTo().Frame(iframe);

                    _driver.FindElement(By.Id("publishAllCheckBox"), 5).Click();
                    Thread.Sleep(1000);

                    _driver.FindElement(By.CssSelector("#container .btn-primary"), 5).Click();
                    var success = _driver.FindElement(By.CssSelector("#feedbackMsg .success"), 10);
                    Assert.IsNotNull(success, "Failed to publish.");

                    _driver.FindElement(By.CssSelector("#feedbackMsg .btn"), 5).Click();

                    _driver.SwitchTo().ParentFrame();

                    Thread.Sleep(1000);

                    builder = new Actions(_driver);
                    builder.ContextClick(node).Perform();
                    Thread.Sleep(1000);

                    menu = _driver.FindElement(By.Id("contextMenu"), 5);
                    menu.FindElement(By.PartialLinkText("Culture and Hostnames")).Click();
                    iframe = _driver.FindElement(By.CssSelector("#dialog iframe"), 10);
                    _driver.SwitchTo().Frame(iframe);
                    Thread.Sleep(5000);

                    _driver.FindElement(By.CssSelector(".umb-dialog-body .btn")).Click();
                    _driver.FindElement(By.ClassName("domain"), 5).SendKeys(Config.Site2BaseURL.TrimEnd('/'));
                    _driver.FindElement(By.Id("btnSave")).Click();

                    Thread.Sleep(1000);
                    break;
                }
            }
        }

        private bool LoginToUmbraco(string userName, string password, bool throwOnError = true)
        {
            if (string.IsNullOrEmpty(userName))
            {
                userName = Config.AdminUsername;
            }

            if (string.IsNullOrEmpty(password))
            {
                password = Config.AdminPassword;
            }

            _driver.Navigate().GoToUrl(Config.Site1BaseURL + "umbraco");
            _driver.FindElement(By.Name("username"), 10).SendKeys(userName);
            _driver.FindElement(By.Name("password"), 1).SendKeys(password);
            _driver.FindElement(By.CssSelector("button[type=\"submit\"]")).Click();

            var loginLogo = _driver.FindElement(By.ClassName("avatar"), 30);

            if (loginLogo == null && throwOnError)
            {
                Assert.IsNotNull(loginLogo, "Umbraco dashboard not loaded.");
            }

            return loginLogo != null;
        }

        private void EnableGigyaForAdmin()
        {
            _driver.Navigate().GoToUrl(Config.Site1BaseURL + "umbraco#/users/framed/%252Fumbraco%252Fusers%252FeditUser.aspx%253Fid%253D0");
            _driver.SwitchTo().DefaultContent();
            _driver.FindElement(By.Id("right"), 30);
            _driver.SwitchTo().Frame("right");

            var gigyaSection = _driver.FindElementFromLabel("Gigya", 10);
            if (string.IsNullOrEmpty(gigyaSection.GetAttribute("checked")))
            {
                gigyaSection.Click();
            }
            
            _driver.FindElement(By.Id("body_save")).Click();
            Thread.Sleep(5000);
            _driver.Navigate().Refresh();
            Thread.Sleep(10000);
        }
        
        public void EnterAllGigyaSettings()
        {
            _driver.Navigate().GoToUrl(Config.Site1BaseURL + "umbraco#/gigya/gigyaTree/edit/-1");
            Thread.Sleep(5000);
            _driver.Navigate().Refresh();
            Thread.Sleep(5000);

            var model = new GigyaConfigSettings
            {
                ApiKey = Config.Site1ApiKey,
                ApplicationKey = Config.Site1ApplicationKey,
                ApplicationSecret = Config.Site1ApplicationSecret,
                DataCenter = Config.Site1DataCenter,
                LanguageFallback = Config.Site1LangFallback
            };

            EnterGigyaSettings(model);

            model = new GigyaConfigSettings
            {
                ApiKey = Config.Site2ApiKey,
                ApplicationKey = Config.Site2ApplicationKey,
                ApplicationSecret = Config.Site2ApplicationSecret,
                DataCenter = Config.Site2DataCenter,
                LanguageFallback = Config.Site2LangFallback
            };

            var tree = _driver.FindElement(By.CssSelector("#tree"), 5);
            tree.FindElement(By.PartialLinkText("Home (1)")).Click();

            _driver.FindElement(By.Id("Inherited"), 10).Click();
            
            EnterGigyaSettings(model);
        }

        private void EnterGigyaSettings(GigyaConfigSettings settings)
        {
            _driver.FindElement(By.Id("api-key"), 10).ClearAndSendKeys(settings.ApiKey);
            _driver.FindElement(By.Id("application-key")).ClearAndSendKeys(settings.ApplicationKey);
            IWebElement applicationSecretLabel = null;

            var applicationSecret = _driver.FindElement(By.Id("application-secret"), 2);
            if (applicationSecret == null || !applicationSecret.Displayed)
            {
                applicationSecretLabel = _driver.FindElement(By.XPath("//label[@for='application-secret']"));
                var buttons = applicationSecretLabel.FindElement(By.XPath("..")).FindElements(By.TagName("button"));
                foreach (var button in buttons)
                {
                    if (button.Displayed)
                    {
                        // edit button is only one visible
                        button.Click();
                        applicationSecret = _driver.FindElement(By.Id("application-secret"), 2);
                        break;
                    }
                }
            }

            applicationSecret.ClearAndSendKeys(settings.ApplicationSecret);
            _driver.FindElement(By.Id("data-center")).SendKeys(settings.DataCenter);
            _driver.FindElement(By.Id("language-fallback")).SendKeys(settings.LanguageFallback);
            _driver.FindElement(By.CssSelector("button[type=\"submit\"]")).Click();

            var successAlert = _driver.FindElement(By.ClassName("alert-success"), 10);
            Assert.IsNotNull(successAlert, "Failed to save settings.");

            // wait for speech bubble to hide
            Thread.Sleep(7000);

            // refresh and make sure application secret is masked
            _driver.Navigate().Refresh();

            applicationSecretLabel = _driver.FindElement(By.XPath("//label[@for='application-secret']"), 10);
            var applicationSecretValue = applicationSecretLabel.FindElement(By.XPath("..")).FindElement(By.CssSelector("span")).Text;
            var nonEncryptedValues = applicationSecretValue.Replace("*", string.Empty);

            Assert.AreEqual(nonEncryptedValues.Length, 4, "Should only be 4 non encrypted values.");

            // test invalid settings shows alert
            var saveButton = _driver.FindElement(By.CssSelector("button[type=\"submit\"]"));
            TestErrorShownForInvalidSetting(saveButton, _driver.FindElement(By.Id("api-key"), 10));
            TestErrorShownForInvalidSetting(saveButton, _driver.FindElement(By.Id("application-key"), 10));

            applicationSecret = _driver.FindElement(By.Id("application-secret"), 2);
            if (applicationSecret == null || !applicationSecret.Displayed)
            {
                applicationSecretLabel = _driver.FindElement(By.XPath("//label[@for='application-secret']"));
                var buttons = applicationSecretLabel.FindElement(By.XPath("..")).FindElements(By.TagName("button"));
                foreach (var button in buttons)
                {
                    if (button.Displayed)
                    {
                        // edit button is only one visible
                        button.Click();
                        applicationSecret = _driver.FindElement(By.Id("application-secret"), 2);
                        break;
                    }
                }
            }

            applicationSecret.ClearAndSendKeys(settings.ApplicationSecret);
            SaveSettingsAndCheckForError(saveButton);

            var dcs = new string[] { "EU", "RU", "US" };
            var invalidDc = dcs.First(i => i != Config.Site1DataCenter);
            _driver.FindElement(By.Id("data-center")).SendKeys(invalidDc);
            SaveSettingsAndCheckForError(saveButton);
        }

        private void SaveSettingsAndCheckForError(IWebElement saveButton)
        {
            saveButton.Click();
            var errorAlert = _driver.FindElement(By.CssSelector(".alert-error .close"), 10);
            Assert.IsNotNull(errorAlert, "Error not shown when saving settings.");

            errorAlert.Click();
            Thread.Sleep(500);
        }

        private void TestErrorShownForInvalidSetting(IWebElement saveButton, IWebElement field)
        {
            field.ClearAndSendKeys("aaaaaaa");
            SaveSettingsAndCheckForError(saveButton);
        }

        private void InstallPackage()
        {
            _driver.Navigate().GoToUrl(Config.Site1BaseURL + "umbraco#/developer/framed/%252Fumbraco%252Fdeveloper%252Fpackages%252Finstaller.aspx");
            Thread.Sleep(5000);

            _driver.FindElement(By.Id("right"), 10);
            _driver.SwitchTo().Frame("right");
            _driver.FindElement(By.Id("cb")).Click();

            if (!File.Exists(Config.UmbracoPackagePath))
            {
                Assert.Fail("Umbraco package not found.");
            }

            _driver.FindElement(By.Id("body_file1")).SendKeys(Config.UmbracoPackagePath);
            _driver.FindElement(By.Id("body_ButtonLoadPackage")).Click();
            Thread.Sleep(5000);

            // check if package already installed
            var alerts = _driver.FindElements(By.ClassName("alert-error"));
            if (alerts.Count > 1)
            {
                // should only be one alert
                return;
            }

            // accept license
            _driver.FindElement(By.Id("body_acceptCheckbox"), 30).Click();
            _driver.FindElement(By.Id("body_ButtonInstall")).Click();

            Thread.Sleep(60000);
        }

        private bool CreateUser()
        {
            var newsletterField = _driver.FindElement(By.Id("subscribeToNewsLetter"), 5);
            if (newsletterField == null)
            {
                return false;
            }

            _driver.FindElement(By.Id("name"), 20).SendKeys(Config.AdminFirstName + " " + Config.AdminLastName);
            _driver.FindElement(By.Id("email"), 0).SendKeys(Config.AdminEmail);
            _driver.FindElement(By.Id("password"), 0).SendKeys(Config.AdminPassword);
            newsletterField.Click();
            Thread.Sleep(100);
            _driver.FindElement(By.CssSelector("input[type=\"submit\"]")).Click();

            var loginLogo = _driver.FindElement(By.ClassName("avatar"), 120);
            Assert.IsNotNull(loginLogo, "Umbraco dashboard not loaded.");

            return true;
        }
    }

    public class GigyaConfigSettings
    {
        public string ApiKey { get; set; }
        public string ApplicationKey { get; set; }
        public string ApplicationSecret { get; set; }
        public string DataCenter { get; set; }
        public string LanguageFallback { get; set; }
    }
}
