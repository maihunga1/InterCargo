using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace InterCargo.UITests.Utilities
{
    public abstract class BaseTest : IDisposable
    {
        protected IWebDriver Driver { get; private set; }
        protected string BaseUrl { get; } = "http://localhost:5272";

        protected BaseTest()
        {
            var options = new ChromeOptions();
            // options.AddArgument("--headless"); // Run in headless mode
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--allow-insecure-localhost");

            Driver = new ChromeDriver(options);
            Driver.Manage().Window.Maximize();
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
        }

        protected void NavigateToUrl(string path)
        {
            Driver.Navigate().GoToUrl($"http://localhost:5272{path}");
        }

        public void Dispose()
        {
            Driver?.Quit();
            Driver?.Dispose();
        }

        protected IWebElement WaitForElement(By by, int timeoutSeconds = 20)
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));

            try
            {
                return wait.Until(driver =>
                {
                    var element = driver.FindElement(by);
                    return element.Displayed && element.Enabled ? element : null;
                });
            }
            catch (WebDriverTimeoutException)
            {
                return null;
            }
        }
    }
}