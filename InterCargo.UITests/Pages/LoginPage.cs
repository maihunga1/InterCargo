using OpenQA.Selenium;

namespace InterCargo.UITests.Pages
{
    public class LoginPage : BasePage
    {
        private By ErrorMessage => By.CssSelector(".alert-danger, .validation-summary-errors, .text-danger");
        private By DashboardElement => By.CssSelector("#dashboardElement"); // Replace with a unique element on the dashboard

        public LoginPage(IWebDriver driver) : base(driver)
        {
        }

        public void NavigateToLoginPage()
        {
            NavigateToUrl("/Users/LoginUser");
        }

        public void FillLoginForm(string emailOrUsername, string password)
        {
            if (!string.IsNullOrEmpty(emailOrUsername))
            {
                WaitForElement(By.Id("Input_EmailOrUsername")).SendKeys(emailOrUsername);
            }
            if (!string.IsNullOrEmpty(password))
            {
                WaitForElement(By.Id("Input_Password")).SendKeys(password);
            }
        }

        public void SubmitLogin()
        {
            WaitForElement(By.CssSelector("button[type='submit']")).Click();
        }

        public void ClickRegisterLink()
        {
            WaitForElement(By.LinkText("Register here")).Click();
        }

        public bool IsErrorMessageDisplayed()
        {
            try
            {
                return IsElementDisplayed(ErrorMessage);
            }
            catch
            {
                return false;
            }
        }

        public string GetErrorMessage()
        {
            try
            {
                return GetText(ErrorMessage);
            }
            catch
            {
                return string.Empty;
            }
        }

        public bool IsSuccessMessageDisplayed()
        {
            try
            {
                return IsElementDisplayed(By.CssSelector(".alert-success"));
            }
            catch
            {
                return false;
            }
        }

        public string GetSuccessMessage()
        {
            try
            {
                return GetElementText(By.CssSelector(".alert-success"));
            }
            catch
            {
                return string.Empty;
            }
        }

        public bool IsLoginSuccessful()
        {
            try
            {
                return Driver.Url.Contains("/Users/Dashboard") || IsElementDisplayed(DashboardElement);
            }
            catch
            {
                return false;
            }
        }
    }
}