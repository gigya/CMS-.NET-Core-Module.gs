﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.IO;
using System.Threading;
using OpenQA.Selenium.Interactions;
using System.Xml;
using System.Xml.Linq;

namespace Gigya.UnitTests.Selenium
{
    [TestClass]
    public class UmbracoInstallationTests
    {
        private IWebDriver _driver;
        
        [TestInitialize]
        public void SetupTest()
        {
            _driver = new FirefoxDriver();
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
        public void CanSetupNewUmbracoSite()
        {
            _driver.Navigate().GoToUrl(Config.Site1BaseURL);

            CreateUser();
            InstallPackage();

            //LoginToUmbraco();
            EnableGigyaForAdmin();
            AddGigyaSettingsToMaster();
            EnableMacrosOnHomepage();
            AddMacrosToHomepage();
            CopyHomepage();
            AddEncryptionKey();
            EnterAllGigyaSettings();
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

            _driver.FindElement(By.CssSelector(".uSky-templates-rows .preview-row .preview-col"), 10).Click();
            _driver.FindElement(By.CssSelector(".usky-grid-configuration .uSky-templates-column"), 10).Click();
            _driver.FindElement(By.XPath("//localize[@key='grid_allowAllEditors']"), 5).Click();
            _driver.FindElement(By.CssSelector(".umb-panel-footer .btn-primary")).Click();
            _driver.FindElement(By.ClassName("btn-success"), 5).Click();
            Thread.Sleep(5000);
        }

        private void AddMacroToHomepage(string macroName)
        {
            _driver.FindElement(By.CssSelector(".cell-tools-add .iconBox"), 10).Click();
            Thread.Sleep(1000);
            _driver.FindElement(By.PartialLinkText("Macro"), 5).Click();
            var modal = _driver.FindElement(By.Name("insertMacroForm"), 5);

            var select = modal.FindElement(By.TagName("select"));
            select.SendKeys(macroName);
            select.SendKeys(Keys.Tab);

            Thread.Sleep(2000);
            modal.FindElement(By.ClassName("btn-primary")).Click();
            Thread.Sleep(2000);
        }

        private void AddMacrosToHomepage()
        {
            _driver.Navigate().GoToUrl(Config.Site1BaseURL + "umbraco#/content/content/edit/1063");

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

        private void LoginToUmbraco()
        {
            _driver.Navigate().GoToUrl(Config.Site1BaseURL + "umbraco");
            _driver.FindElement(By.Name("username"), 10).SendKeys(Config.AdminEmail);
            _driver.FindElement(By.Name("password"), 1).SendKeys(Config.AdminPassword);
            _driver.FindElement(By.CssSelector("button[type=\"submit\"]")).Click();

            var loginLogo = _driver.FindElement(By.ClassName("avatar"), 30);
            Assert.IsNotNull(loginLogo, "Umbraco dashboard not loaded.");
        }

        private void EnableGigyaForAdmin()
        {
            _driver.Navigate().GoToUrl(Config.Site1BaseURL + "umbraco#/users/framed/%252Fumbraco%252Fusers%252FeditUser.aspx%253Fid%253D0");
            _driver.FindElement(By.Id("right"), 10);
            _driver.SwitchTo().Frame("right");

            var gigyaSection = _driver.FindElementFromLabel("[gigya]", 10);
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
            _driver.Navigate().Refresh();

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

        private void AddEncryptionKey()
        {
            XmlDocument webConfigDoc = new XmlDocument();
            webConfigDoc.Load(Path.Combine(Config.UmbracoRootPath, "web.config"));

            // add encryption key to web.config
            var encryptionElem = webConfigDoc.SelectSingleNode("/configuration/appSettings/add[@key='Gigya.Encryption.Key']");
            if (encryptionElem == null)
            {
                var appSettings = webConfigDoc.SelectSingleNode("/configuration/appSettings");

                encryptionElem = webConfigDoc.CreateElement("add");
                var keyAttribute = webConfigDoc.CreateAttribute("key");
                keyAttribute.Value = "Gigya.Encryption.Key";

                var valueAttribute = webConfigDoc.CreateAttribute("value");
                valueAttribute.Value = "secret";

                encryptionElem.Attributes.Append(keyAttribute);
                encryptionElem.Attributes.Append(valueAttribute);

                appSettings.AppendChild(encryptionElem);

                webConfigDoc.Save(Path.Combine(Config.UmbracoRootPath, "web.config"));
            }
        }

        private void EnterGigyaSettings(GigyaConfigSettings settings)
        {
            _driver.FindElement(By.Id("api-key"), 10).ClearAndSendKeys(settings.ApiKey);
            _driver.FindElement(By.Id("application-key")).ClearAndSendKeys(settings.ApplicationKey);


            var applicationSecret = _driver.FindElement(By.Id("application-secret"), 2);
            if (applicationSecret == null || !applicationSecret.Displayed)
            {
                var applicationSecretLabel = _driver.FindElement(By.XPath("//label[@for='application-secret']"));
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
            
            // accept license
            _driver.FindElement(By.Id("body_acceptCheckbox"), 30).Click();
            _driver.FindElement(By.Id("body_ButtonInstall")).Click();

            Thread.Sleep(60000);
        }

        private void CreateUser()
        {
            _driver.FindElement(By.Id("name"), 20).SendKeys(Config.AdminFirstName + " " + Config.AdminLastName);
            _driver.FindElement(By.Id("email"), 0).SendKeys(Config.AdminEmail);
            _driver.FindElement(By.Id("password"), 0).SendKeys(Config.AdminPassword);
            _driver.FindElement(By.Id("subscribeToNewsLetter")).Click();
            Thread.Sleep(100);
            _driver.FindElement(By.CssSelector("input[type=\"submit\"]")).Click();

            var loginLogo = _driver.FindElement(By.ClassName("avatar"), 120);
            Assert.IsNotNull(loginLogo, "Umbraco dashboard not loaded.");
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