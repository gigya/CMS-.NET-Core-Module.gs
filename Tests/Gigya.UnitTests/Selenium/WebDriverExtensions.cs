using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.UnitTests.Selenium
{
    public static class WebDriverExtensions
    {
        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));

                try
                {
                    return wait.Until(ExpectedConditions.ElementToBeClickable(by));
                }
                catch
                {
                    return null;
                }
            }
            return driver.FindElement(by);
        }

        public static IWebElement FindElementFromLabel(this IWebDriver driver, string labelText, int timeout = 0)
        {
            var label = driver.FindLabel(labelText, timeout);
            return driver.FindElement(By.Id(label.GetAttribute("for")));
        }

        public static IWebElement FindLabel(this IWebDriver driver, string labelText, int timeout = 0)
        {
            var label = driver.FindElement(By.XPath("//*[translate(normalize-space(text()), ' ', '') = '" + labelText + "']"), timeout);
            if (label != null)
            {
                return label;
            }

            var labels = driver.FindElements(By.XPath(string.Format("//*[contains(text(), '{0}')]", labelText)));
            foreach (var l in labels)
            {
                if (l.Text.Trim().StartsWith(labelText))
                {
                    return l;
                }
            }

            return null;
        }

        public static IWebElement ClearAndSendKeys(this IWebElement element, string text)
        {
            element.Clear();
            element.SendKeys(text);
            return element;
        }

        public static IWebElement ClearWithBackspaceAndSendKeys(this IWebElement element, string text)
        {
            var value = element.GetAttribute("value");
            element.SendKeys(Keys.End);

            for (int i = 0; i < value.Length; i++)
            {
                element.SendKeys(Keys.Backspace);
            }

            element.SendKeys(text);
            return element;
        }
    }

}
