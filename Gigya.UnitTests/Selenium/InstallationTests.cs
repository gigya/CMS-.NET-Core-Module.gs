using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.IO;
using System.Threading;
using OpenQA.Selenium.Interactions;

namespace Gigya.UnitTests.Selenium
{
    [TestClass]
    public class InstallationTests
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
        public void Sitefinity_CanSetupNewSitefinitySite()
        {
            Utils.AddEncryptionKey(Config.SitefinityRootPath);

            _driver.Navigate().GoToUrl(Config.Site1BaseURL);

            UploadLicense();
            SelectDb();
            EnterAdminDetails();

            SitefinityUtils.LoginToSitefinity(_driver, 300);

            _driver.SwitchTo().DefaultContent();
            CreateHomepage();

            AddGigyaSettings();
            CreateSecondSite();

            _driver.Navigate().GoToUrl(Config.Site1BaseURL + "Sitefinity");
            Thread.Sleep(5000);
            _driver.FindElement(By.PartialLinkText("Logout"), 5).Click();
            Thread.Sleep(10000);

            SitefinityUtils.LoginToSitefinity(_driver, 20);

            AddGigyaSettingsForSecondSite();
        }

        private void CreateSecondSite()
        {
            _driver.Navigate().GoToUrl(Config.Site1BaseURL + "Sitefinity/MultisiteManagement");
            _driver.FindElement(By.PartialLinkText("Create a site"), 10).Click();

            var form = _driver.FindElement(By.ClassName("sfFirstForm"), 5);
            var textFields = form.FindElements(By.ClassName("sfTxt"));
            textFields[0].SendKeys("Gigya 2");
            textFields[1].SendKeys(Config.Site2BaseURL);
            _driver.FindElementFromLabel("Duplicate pages and settings from existing site...", 5).Click();

            Thread.Sleep(1000);

            _driver.FindElement(By.CssSelector(".sfButtonArea.sfMainFormBtns .sfLinkBtn.sfPrimary"), 2).Click();

            Thread.Sleep(5000);

            _driver.FindElement(By.PartialLinkText("Create this site"), 5).Click();

            Assert.IsNotNull(_driver.FindElement(By.PartialLinkText("Logout"), 30));
        }

        private void AddGigyaSettingsForSecondSite()
        {
            _driver.Navigate().GoToUrl(Config.Site1BaseURL + "Sitefinity");
            Thread.Sleep(3000);

            _driver.FindElement(By.CssSelector(".sfSiteSelectorMenuWrp .clickMenu .main"), 5).Click();
            Thread.Sleep(1000);

            var gigya2Site = _driver.FindElement(By.PartialLinkText("Gigya 2"), 5);
            if (gigya2Site == null)
            {
                // try again
                _driver.FindElement(By.CssSelector(".sfSiteSelectorMenuWrp .clickMenu .main"), 5).Click();
                Thread.Sleep(1000);
            }
            
            gigya2Site.Click();

            Thread.Sleep(5000);

            _driver.Navigate().GoToUrl(Config.Site1BaseURL + "Sitefinity/Administration/Settings/Basic/GigyaModule/");

            var loginUsername = _driver.FindElement(By.Id("wrap_name"), 30); ;
            if (loginUsername != null)
            {
                SitefinityUtils.LoginToSitefinity(_driver, 300);
                _driver.Navigate().GoToUrl(Config.Site1BaseURL + "Sitefinity/Administration/Settings/Basic/GigyaModule/");
            }

            var saveButton = _driver.FindElement(By.ClassName("sfSave"), 5);

            // enter valid settings
            _driver.FindElementFromLabel("API Key", 5).ClearWithBackspaceAndSendKeys(Config.Site2ApiKey);
            _driver.FindElementFromLabel("Application Key", 2).ClearWithBackspaceAndSendKeys(Config.Site2ApplicationKey);
            _driver.FindElement(By.Id("edit-application-secret")).Click();
            _driver.FindElementFromLabel("Application Secret", 2).ClearWithBackspaceAndSendKeys(Config.Site2ApplicationSecret);

            _driver.FindElement(By.CssSelector("#data-center-wrapper select"), 2).SendKeys(Config.Site2DataCenter);
            saveButton.Click();

            var positiveMessage = _driver.FindElement(By.ClassName("sfMsgPositive"), 2);
            Assert.IsNotNull(positiveMessage, "Settings not saved. Check they are correct.");
        }
        
        private void AddGigyaSettings()
        {
            _driver.Navigate().GoToUrl(Config.Site1BaseURL + "Sitefinity/Administration/Settings/Basic/GigyaModule/");

            var loginUsername = _driver.FindElement(By.Id("wrap_name"), 30); ;
            if (loginUsername != null)
            {
                SitefinityUtils.LoginToSitefinity(_driver, 300);
                _driver.Navigate().GoToUrl(Config.Site1BaseURL + "Sitefinity/Administration/Settings/Basic/GigyaModule/");
            }

            var saveButton = _driver.FindElement(By.ClassName("sfSave"), 5);
            
            // enter valid settings
            _driver.FindElementFromLabel("API Key", 5).ClearWithBackspaceAndSendKeys(Config.Site1ApiKey);
            _driver.FindElementFromLabel("Application Key", 2).ClearWithBackspaceAndSendKeys(Config.Site1ApplicationKey);
            _driver.FindElement(By.Id("edit-application-secret")).Click();
            _driver.FindElementFromLabel("Application Secret", 2).ClearWithBackspaceAndSendKeys(Config.Site1ApplicationSecret);
            
            _driver.FindElement(By.CssSelector("#data-center-wrapper select"), 2).SendKeys(Config.Site1DataCenter);
            saveButton.Click();

            var positiveMessage = _driver.FindElement(By.ClassName("sfMsgPositive"), 2);
            Assert.IsNotNull(positiveMessage, "Settings not saved. Check they are correct.");

            // test invalid settings
            EnterInvalidSettingAndWaitForAlert(_driver.FindElementFromLabel("API Key", 2), saveButton, Config.Site1ApiKey);
            EnterInvalidSettingAndWaitForAlert(_driver.FindElementFromLabel("Application Key", 2), saveButton, Config.Site1ApplicationKey);
            EnterInvalidSettingAndWaitForAlert(_driver.FindElementFromLabel("Application Secret", 2), saveButton, Config.Site1ApplicationSecret);

            var dcs = new string[] { "EU", "RU", "US" };
            var invalidDc = dcs.First(i => i != Config.Site1DataCenter);

            _driver.FindElement(By.CssSelector("#data-center-wrapper select"), 2).SendKeys(invalidDc);
            saveButton.Click();
            Thread.Sleep(5000);
            _driver.SwitchTo().Alert().Dismiss();

            _driver.Navigate().Refresh();

            var applicationSecretValue = _driver.FindElement(By.CssSelector(".application-secret-masked .sfTxtContent"), 5).Text;
            var nonEncryptedValues = applicationSecretValue.Replace("*", string.Empty);

            Assert.AreEqual(nonEncryptedValues.Length, 4, "Should only be 4 non encrypted values.");
        }

        private void EnterInvalidSettingAndWaitForAlert(IWebElement element, IWebElement saveButton, string correctValue)
        {
            element.ClearWithBackspaceAndSendKeys("adsfasdf");
            saveButton.Click();
            Thread.Sleep(5000);
            _driver.SwitchTo().Alert().Dismiss();

            element.ClearWithBackspaceAndSendKeys(correctValue);
        }
        
        private void CreateHomepage()
        {
            _driver.Navigate().GoToUrl(Config.Site1BaseURL + "Sitefinity/Pages");

            _driver.FindElement(By.PartialLinkText("Create a page"), 30).Click();

            _driver.SwitchTo().Frame("create");
            _driver.FindElementFromLabel("Name", 20).SendKeys("Home");

            _driver.FindElement(By.LinkText("Create and go to add content"), 1).Click();

            _driver.SwitchTo().DefaultContent();
            var publishButton = _driver.FindElement(By.LinkText("Publish"), 30);

            var headerPlaceholder = _driver.FindElement(By.XPath("//div[@data-placeholder-label='Header']"), 5);

            // drag gigya settings widget
            var gigyaSection = _driver.FindElement(By.LinkText("Gigya"), 1);
            gigyaSection.Click();

            Thread.Sleep(1000);

            var gigyaWidgets = _driver.FindElements(By.CssSelector(".sf_gigya.sfMvcIcn"));

            foreach (var widget in gigyaWidgets)
            {
                DragGigyaWidget(headerPlaceholder, widget);
            }

            Thread.Sleep(5000);

            // publish
            publishButton.Click();

            _driver.FindElement(By.Id("MainMenu"), 20);
        }

        private void DragGigyaWidget(IWebElement headerPlaceholder, IWebElement gigyaWidget)
        {
            Actions builder = new Actions(_driver);
            builder.ClickAndHold(gigyaWidget);
            builder.Build().Perform();
            Thread.Sleep(2000);

            builder.MoveToElement(headerPlaceholder);

            // move up and along a bit to hit the drop target
            builder.MoveByOffset(50, -10);//
            Thread.Sleep(2000);
            builder.Release().Build().Perform();

            Thread.Sleep(1000);
        }

        private void EnterAdminDetails()
        {
            _driver.FindElement(By.Id("wizard_ctl00_ctl07_FirstName"), 30).ClearAndSendKeys(Config.AdminFirstName);
            _driver.FindElement(By.Id("wizard_ctl00_ctl07_LastName")).ClearAndSendKeys(Config.AdminLastName);
            _driver.FindElement(By.Id("wizard_ctl00_ctl07_Username")).ClearAndSendKeys(Config.AdminUsername);
            _driver.FindElement(By.Id("wizard_ctl00_ctl07_Password")).ClearAndSendKeys(Config.AdminPassword);
            _driver.FindElement(By.Id("wizard_ctl00_ctl07_RePassword")).ClearAndSendKeys(Config.AdminPassword);
            _driver.FindElement(By.Id("wizard_ctl00_ctl07_Email")).ClearAndSendKeys(Config.AdminEmail);
            _driver.FindElement(By.Id("wizard_ctl00_ctl10_FinishButton")).Click();

            // wait about 5 mins for dashboard
            _driver.FindElement(By.Id("MainMenu"), 300);
        }

        private void SelectDb()
        {
            _driver.FindElement(By.Id("wizard_ctl00_ctl04_SqlExpress"), 30).Click();
            _driver.FindElement(By.Id("wizard_ctl00_ctl10_StepNextButton"), 5).Click();
        }

        private void UploadLicense()
        {
            // wait the usual 1 min for sitefinity to start up....
            var licenseRadio = _driver.FindElement(By.Id("lic_ctl00_ctl00_rbManualMode"), 30);
            if (licenseRadio == null)
            {
                return;
            }
            
            licenseRadio.Click();

            var licensePath = Config.LicensePath;
            if (string.IsNullOrEmpty(licensePath))
            {
                // assume it's in current directory
                licensePath = Path.Combine(Directory.GetCurrentDirectory(), "Sitefinity.lic");
            }

            var fileUpload = _driver.FindElement(By.Id("lic_ctl00_ctl00_fuLicenseFile"), 5);
            fileUpload.SendKeys(licensePath);

            // upload license
            _driver.FindElement(By.Id("lic_ctl00_ctl00_btnUploadFile")).Click();

            _driver.FindElement(By.Id("lic_ctl00_ctl00_confirmWindow_C_btnContinueWithUpdate"), 10).Click();
        }
    }
}
