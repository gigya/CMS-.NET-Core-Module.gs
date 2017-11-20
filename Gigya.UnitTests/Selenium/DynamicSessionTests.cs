using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gigya.UnitTests.Selenium.Authentication;
using System.Threading;
using OpenQA.Selenium;

namespace Gigya.UnitTests.Selenium
{
    [TestClass]
    public class DynamicSessionTests : SitefinityRegisterLoginTests
    {
        [TestMethod]
        public void IsSessionExtended()
        {
            // login to front end
            _newEmail = "jason.king@purestone.co.uk";
            _passsword = "aa234567";

            CanLoginToFrontEnd();

            // wait 120 seconds
            Thread.Sleep(120000);

            // refresh browser
            _driver.Navigate().Refresh();

            // should still be logged in
            var logoutButton = _driver.FindElement(By.ClassName("gigya-logout"), 10);
            Assert.IsNotNull(logoutButton, "Logout button not found. User should be logged in.");
        }
    }
}
