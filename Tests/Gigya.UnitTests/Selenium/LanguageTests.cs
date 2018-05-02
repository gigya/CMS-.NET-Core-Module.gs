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
    public class LanguageTests
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

        //[TestMethod]
        public void SitefinityFrenchLanguageMappedToGigya()
        {

        }
    }
}
