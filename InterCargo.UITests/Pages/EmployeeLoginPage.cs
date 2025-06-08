using OpenQA.Selenium;

namespace InterCargo.UITests.Pages
{
    public class EmployeeLoginPage : BasePage
    {
        public EmployeeLoginPage(IWebDriver driver) : base(driver)
        {
        }

        public void NavigateToLoginPage()
        {
            NavigateToUrl("/Employees/LoginEmployee");
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
                return IsElementDisplayed(By.CssSelector(".alert-danger")) ||
                       IsElementDisplayed(By.CssSelector(".validation-summary-errors")) ||
                       IsElementDisplayed(By.CssSelector(".text-danger"));
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
                if (IsElementDisplayed(By.CssSelector(".alert-danger")))
                {
                    return GetElementText(By.CssSelector(".alert-danger"));
                }
                if (IsElementDisplayed(By.CssSelector(".validation-summary-errors")))
                {
                    return GetElementText(By.CssSelector(".validation-summary-errors"));
                }
                if (IsElementDisplayed(By.CssSelector(".text-danger")))
                {
                    return GetElementText(By.CssSelector(".text-danger"));
                }
                return string.Empty;
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
                // Check if we're redirected to the quotations page
                if (Driver.Url.Contains("/Quotations/Confirm"))
                {
                    return true;
                }

                // Check for success message
                if (IsElementDisplayed(By.CssSelector(".alert-success")))
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}