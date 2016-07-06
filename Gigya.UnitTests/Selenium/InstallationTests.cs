using System;
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
        public void CanSetupNewSitefinitySite()
        {
            _driver.Navigate().GoToUrl(Config.Site1BaseURL);

            UploadLicense();
            SelectDb();
            EnterAdminDetails();
            SitefinityUtils.LoginToSitefinity(_driver, 300);
            CreateHomepage();
        }
        
        private void CreateHomepage()
        {
            _driver.Navigate().GoToUrl(Config.Site1BaseURL + "Sitefinity/Pages");

            _driver.FindElement(By.Id("ctl04_frontendPagesListView_ctl00_ctl00_noItemsExistScreen_ctl00_ctl00_actionsRepeater_actionItem_0"), 30).Click();

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

            DragGigyaWidget(headerPlaceholder, gigyaWidgets[0]);
            DragGigyaWidget(headerPlaceholder, gigyaWidgets[1]);

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
