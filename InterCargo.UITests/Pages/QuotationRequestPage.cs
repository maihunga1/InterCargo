using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace InterCargo.UITests.Pages
{
    public class QuotationRequestPage : BasePage
    {
        public QuotationRequestPage(IWebDriver driver) : base(driver)
        {
        }

        // Locators
        private By SourceInput => By.Id("Input_Source");
        private By DestinationInput => By.Id("Input_Destination");
        private By NumberOfContainersInput => By.Id("Input_NumberOfContainers");
        private By ContainerTypeSelect => By.Id("Input_ContainerType");
        private By PackageNatureInput => By.Id("Input_PackageNature");
        private By ImportExportTypeSelect => By.Id("Input_ImportExportType");
        private By PackingUnpackingRadio(string type) => By.Id($"requires{type}");
        private By QuarantineRequirementsInput => By.Id("Input_QuarantineRequirements");
        private By SubmitButton => By.Id("submitButton");
        private By SuccessMessage => By.CssSelector(".alert-success");
        private By ErrorMessage => By.CssSelector(".alert-danger, .validation-summary-errors, .text-danger");

        public void NavigateToRequestPage()
        {
            NavigateToUrl("/Quotations/Submit");
        }

        public void FillQuotationRequest(
            string source,
            string destination,
            int numberOfContainers,
            string containerType,
            string packageNature,
            string importExportType,
            string packingUnpackingType,
            string quarantineRequirements)
        {
            if (!string.IsNullOrEmpty(source))
            {
                SendKeys(SourceInput, source);
            }

            if (!string.IsNullOrEmpty(destination))
            {
                SendKeys(DestinationInput, destination);
            }

            if (numberOfContainers > 0)
            {
                SendKeys(NumberOfContainersInput, numberOfContainers.ToString());
            }

            if (!string.IsNullOrEmpty(containerType))
            {
                var containerTypeSelect = new SelectElement(WaitForElement(ContainerTypeSelect));
                containerTypeSelect.SelectByText(containerType);
            }

            if (!string.IsNullOrEmpty(packageNature))
            {
                SendKeys(PackageNatureInput, packageNature);
            }

            if (!string.IsNullOrEmpty(importExportType))
            {
                var importExportTypeSelect = new SelectElement(WaitForElement(ImportExportTypeSelect));
                importExportTypeSelect.SelectByText(importExportType);
            }

            if (!string.IsNullOrEmpty(packingUnpackingType))
            {
                Click(PackingUnpackingRadio(packingUnpackingType));
            }

            if (!string.IsNullOrEmpty(quarantineRequirements))
            {
                SendKeys(QuarantineRequirementsInput, quarantineRequirements);
            }
        }

        public void SubmitRequest()
        {
            try
            {
                Console.WriteLine("Attempting to submit quotation request...");
                var submitButton = WaitForElement(By.Id("submitButton"));
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
                        // Check if we're on a different page
                        return driver.Url.Contains("/Users/Dashboard") ||
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
                Console.WriteLine($"Error submitting request: {ex.Message}");
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