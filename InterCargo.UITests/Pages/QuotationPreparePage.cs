using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace InterCargo.UITests.Pages
{
    public class QuotationPreparePage : BasePage
    {
        public QuotationPreparePage(IWebDriver driver) : base(driver)
        {
        }

        // Locators
        private By ChargeItemCheckbox(string item) => By.CssSelector($"input[type='checkbox'][id*='{item.ToLower().Replace(" ", "-")}']");
        private By DiscountInput => By.Id("discount-input");
        private By FinalPrice => By.Id("final-price");
        private By CustomerName => By.CssSelector(".customer-details .name");
        private By CustomerEmail => By.CssSelector(".customer-details .email");
        private By Source => By.CssSelector(".quotation-details .source");
        private By Destination => By.CssSelector(".quotation-details .destination");
        private By NumberOfContainers => By.CssSelector(".quotation-details .containers");
        private By ContainerType => By.CssSelector(".quotation-details .container-type");
        private By SubmitButton => By.CssSelector("button[type='submit']");
        private By SuccessMessage => By.CssSelector(".alert-success");
        private By ErrorMessage => By.CssSelector(".alert-danger, .validation-summary-errors, .text-danger");

        public void NavigateToPreparePage(string quotationId)
        {
            NavigateToUrl($"/Quotations/Prepare?quotationId={quotationId}");
        }

        public void SelectChargeItem(string item)
        {
            try
            {
                var checkbox = WaitForElement(ChargeItemCheckbox(item));
                if (!checkbox.Selected)
                {
                    Click(ChargeItemCheckbox(item));
                }
            }
            catch
            {
                // Log error or handle gracefully
            }
        }

        public void SetDiscount(decimal discount)
        {
            try
            {
                SendKeys(DiscountInput, discount.ToString());
            }
            catch
            {
                // Log error or handle gracefully
            }
        }

        public string GetCustomerName()
        {
            try
            {
                return GetText(CustomerName);
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GetCustomerEmail()
        {
            try
            {
                return GetText(CustomerEmail);
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GetSource()
        {
            try
            {
                return GetText(Source);
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GetDestination()
        {
            try
            {
                return GetText(Destination);
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GetNumberOfContainers()
        {
            try
            {
                return GetText(NumberOfContainers);
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GetContainerType()
        {
            try
            {
                return GetText(ContainerType);
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GetFinalPrice()
        {
            try
            {
                return GetText(FinalPrice);
            }
            catch
            {
                return "$0.00";
            }
        }

        public void SubmitQuotation()
        {
            try
            {
                Console.WriteLine("Attempting to submit quotation...");
                var submitButton = WaitForElement(SubmitButton);
                Console.WriteLine($"Submit button found: {submitButton.Displayed}, {submitButton.Enabled}");

                // Scroll the button into view
                ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", submitButton);
                Console.WriteLine("Scrolled button into view");

                // Wait for any overlays to disappear
                Thread.Sleep(1000);

                // Try JavaScript click
                ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", submitButton);
                Console.WriteLine("Clicked submit button using JavaScript");

                // Wait for form submission to complete
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
                wait.Until(driver =>
                {
                    try
                    {
                        // Check if we're on a different page or if success message is displayed
                        return driver.Url.Contains("/Quotations/Confirm") ||
                               driver.FindElement(By.CssSelector(".alert-success")).Displayed;
                    }
                    catch
                    {
                        return false;
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error submitting quotation: {ex.Message}");
                Console.WriteLine($"Current URL: {Driver.Url}");
                Console.WriteLine($"Page source: {Driver.PageSource}");
                throw;
            }
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