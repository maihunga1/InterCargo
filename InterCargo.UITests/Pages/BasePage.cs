using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace InterCargo.UITests.Pages
{
    public abstract class BasePage
    {
        public IWebDriver Driver { get; private set; }
        protected readonly WebDriverWait Wait;
        protected string BaseUrl { get; } = "http://localhost:5272";

        protected BasePage(IWebDriver driver)
        {
            Driver = driver;
            Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            Wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
        }

        protected void NavigateToUrl(string relativeUrl)
        {
            var baseUrl = "http://localhost:5272"; // Update this with your actual base URL
            Driver.Navigate().GoToUrl($"{baseUrl}{relativeUrl}");
        }

        protected IWebElement WaitForElement(By locator)
        {
            return Wait.Until(driver => driver.FindElement(locator));
        }

        protected bool IsElementDisplayed(By locator)
        {
            try
            {
                return WaitForElement(locator).Displayed;
            }
            catch
            {
                return false;
            }
        }

        protected void Click(By locator)
        {
            WaitForElement(locator).Click();
        }

        protected void SendKeys(By locator, string text)
        {
            WaitForElement(locator).SendKeys(text);
        }

        protected string GetText(By locator)
        {
            return WaitForElement(locator).Text;
        }

        protected string GetElementText(By locator)
        {
            return WaitForElement(locator).Text;
        }

        protected void SelectByText(By selectLocator, string text)
        {
            var element = WaitForElement(selectLocator);
            var select = new SelectElement(element);
            select.SelectByText(text);
        }
    }
}