using OpenQA.Selenium;

namespace InterCargo.UITests.Pages
{
    public class RegisterPage : BasePage
    {
        public RegisterPage(IWebDriver driver) : base(driver)
        {
        }

        // Locators
        private By UsernameInput => By.Id("Input_Username");
        private By EmailInput => By.Id("Input_Email");
        private By PasswordInput => By.Id("Input_Password");
        private By ConfirmPasswordInput => By.Id("Input_ConfirmPassword");
        private By FirstNameInput => By.Id("Input_FirstName");
        private By FamilyNameInput => By.Id("Input_FamilyName");
        private By PhoneNumberInput => By.Id("Input_PhoneNumber");
        private By CompanyNameInput => By.Id("Input_CompanyName");
        private By AddressInput => By.Id("Input_Address");
        private By RegisterButton => By.CssSelector("button[type='submit']");
        private By SuccessMessage => By.CssSelector(".alert-success");
        private By ErrorMessage => By.CssSelector(".alert-danger, .validation-summary-errors, .text-danger");

        public void NavigateToRegisterPage()
        {
            NavigateToUrl("/Users/RegisterUser");
        }

        public void FillRegistrationForm(
            string username,
            string email,
            string password,
            string confirmPassword,
            string firstName,
            string familyName,
            string phoneNumber,
            string companyName,
            string address)
        {
            if (!string.IsNullOrEmpty(username))
            {
                SendKeys(UsernameInput, username);
            }

            if (!string.IsNullOrEmpty(email))
            {
                SendKeys(EmailInput, email);
            }

            if (!string.IsNullOrEmpty(password))
            {
                SendKeys(PasswordInput, password);
            }

            if (!string.IsNullOrEmpty(confirmPassword))
            {
                SendKeys(ConfirmPasswordInput, confirmPassword);
            }

            if (!string.IsNullOrEmpty(firstName))
            {
                SendKeys(FirstNameInput, firstName);
            }

            if (!string.IsNullOrEmpty(familyName))
            {
                SendKeys(FamilyNameInput, familyName);
            }

            if (!string.IsNullOrEmpty(phoneNumber))
            {
                SendKeys(PhoneNumberInput, phoneNumber);
            }

            if (!string.IsNullOrEmpty(companyName))
            {
                SendKeys(CompanyNameInput, companyName);
            }

            if (!string.IsNullOrEmpty(address))
            {
                SendKeys(AddressInput, address);
            }
        }

        public void SubmitRegistration()
        {
            Click(RegisterButton);
        }

        public bool IsSuccessMessageDisplayed()
        {
            try
            {
                return IsElementDisplayed(SuccessMessage);
            }
            catch
            {
                return false;
            }
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

        public string GetSuccessMessage()
        {
            try
            {
                return GetText(SuccessMessage);
            }
            catch
            {
                return string.Empty;
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
    }
}