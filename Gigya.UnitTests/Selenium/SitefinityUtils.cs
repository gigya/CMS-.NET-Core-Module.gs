using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gigya.UnitTests.Selenium
{
    public class SitefinityUtils
    {
        public static void LoginToSitefinity(IWebDriver driver, int timeout = 0)
        {
            driver.Navigate().GoToUrl(Config.Site1BaseURL + "Sitefinity/");
            driver.FindElement(By.Id("wrap_name"), timeout).Clear();
            driver.FindElement(By.Id("wrap_name")).SendKeys(Config.AdminUsername);

            driver.FindElement(By.Id("wrap_password")).Clear();
            driver.FindElement(By.Id("wrap_password")).SendKeys(Config.AdminPassword);

            driver.FindElement(By.ClassName("sfSave")).Click();

            Thread.Sleep(3000);

            var kickOutOtherUserButton = driver.FindElement(By.Id("ctl04_ctl00_ctl00_ctl00_ctl00_ctl00_selfLogoutButton"), 5);
            if (kickOutOtherUserButton != null)
            {
                kickOutOtherUserButton.Click();

                Thread.Sleep(1000);

                var alert = driver.SwitchTo().Alert();
                alert.Accept();

                Thread.Sleep(5000);
            }


        }
    }
}
