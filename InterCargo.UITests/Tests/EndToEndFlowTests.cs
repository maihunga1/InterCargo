using InterCargo.UITests.Pages;
using InterCargo.UITests.Utilities;
using Xunit;
using System;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System.Linq;

namespace InterCargo.UITests.Tests
{
    public class EndToEndFlowTests : BaseTest
    {
        private readonly RegisterPage _registerPage;
        private readonly LoginPage _loginPage;
        private readonly QuotationRequestPage _quotationRequestPage;
        private readonly RegisterEmployeePage _registerEmployeePage;
        private readonly EmployeeLoginPage _employeeLoginPage;
        private readonly QuotationPreparePage _quotationPreparePage;
        private readonly QuotationConfirmPage _quotationConfirmPage;

        public EndToEndFlowTests()
        {
            _registerPage = new RegisterPage(Driver);
            _loginPage = new LoginPage(Driver);
            _quotationRequestPage = new QuotationRequestPage(Driver);
            _registerEmployeePage = new RegisterEmployeePage(Driver);
            _employeeLoginPage = new EmployeeLoginPage(Driver);
            _quotationPreparePage = new QuotationPreparePage(Driver);
            _quotationConfirmPage = new QuotationConfirmPage(Driver);
        }

        [Fact]
        public void CompleteQuotationFlow_Success()
        {
            // Generate unique identifiers for this test run
            var unique = Guid.NewGuid().ToString("N").Substring(0, 8);
            var customerEmail = $"cust_{unique}@example.com";
            var employeeEmail = $"emp_{unique}@example.com";
            string quotationId = null;

            try
            {
                // --- T2: Register new customer ---
                _registerPage.NavigateToRegisterPage();
                _registerPage.FillRegistrationForm(
                    username: $"cust_{unique}",
                    email: customerEmail,
                    password: "Test123!",
                    confirmPassword: "Test123!",
                    firstName: "CustFirst",
                    familyName: "CustLast",
                    phoneNumber: "1234567890",
                    companyName: "CustCompany",
                    address: "123 Cust St"
                );
                _registerPage.SubmitRegistration();
                Assert.True(_registerPage.IsSuccessMessageDisplayed(), $"Registration failed: {_registerPage.GetErrorMessage()}");
                Console.WriteLine("[T2] Customer registration successful.");

                // --- I1: Login as new customer ---
                _loginPage.NavigateToLoginPage();
                _loginPage.FillLoginForm(customerEmail, "Test123!");
                _loginPage.SubmitLogin();
                Assert.True(_loginPage.IsLoginSuccessful(), $"Login failed: {_loginPage.GetErrorMessage()}");
                Console.WriteLine("[I1] Customer login successful.");
                Thread.Sleep(2000); // Ensure dashboard is loaded

                // --- I2: Submit quotation request ---
                _quotationRequestPage.NavigateToRequestPage();
                _quotationRequestPage.FillQuotationRequest(
                    source: "Sydney",
                    destination: "Melbourne",
                    numberOfContainers: 2,
                    containerType: "20 Feet",
                    packageNature: "Electronics",
                    importExportType: "Import",
                    packingUnpackingType: "Packing",
                    quarantineRequirements: "None"
                );
                _quotationRequestPage.SubmitRequest();
                Console.WriteLine("[I2] Quotation request submitted.");

                // Wait for redirect to dashboard
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
                wait.Until(driver => driver.Url.Contains("/Users/Dashboard"));
                Console.WriteLine("[I2] Redirected to dashboard.");

                // Wait for the new quotation to appear in the dashboard table
                wait.Until(driver =>
                {
                    var rows = driver.FindElements(By.CssSelector(".quotation-row"));
                    return rows.Any(row => row.Text.Contains("Sydney") && row.Text.Contains("Melbourne"));
                });
                Console.WriteLine("[I2] New quotation found in dashboard table.");

                // --- T4: Log out from customer ---
                NavigateToUrl("/Users/Logout");
                Console.WriteLine("[T4] Customer logged out.");

                // --- T3: Register new employee ---
                _registerEmployeePage.NavigateToRegisterPage();
                _registerEmployeePage.FillRegistrationForm(
                    username: $"emp_{unique}",
                    email: employeeEmail,
                    password: "Test123!",
                    confirmPassword: "Test123!",
                    firstName: "EmpFirst",
                    familyName: "EmpLast",
                    phoneNumber: "9876543210",
                    employeeId: $"EID{unique}",
                    employeeType: "Quotation Officer",
                    address: "456 Employee St"
                );
                _registerEmployeePage.SubmitRegistration();
                Assert.True(_registerEmployeePage.IsSuccessMessageDisplayed());
                Console.WriteLine("[T3] Employee registration successful.");

                // --- I3: Login as new employee ---
                _employeeLoginPage.NavigateToLoginPage();
                _employeeLoginPage.FillLoginForm(employeeEmail, "Test123!");
                _employeeLoginPage.SubmitLogin();
                Assert.True(_employeeLoginPage.IsLoginSuccessful(), $"Employee login failed: {_employeeLoginPage.GetErrorMessage()}");
                Console.WriteLine("[I3] Employee login successful.");

                // --- I3: Employee views quotation request ---
                _quotationConfirmPage.NavigateToConfirmPage();
                quotationId = _quotationConfirmPage.GetLatestQuotationId();
                Assert.NotNull(quotationId);
                Console.WriteLine($"[I3] Employee sees quotation request: {quotationId}");

                // Declare truncatedId once after quotationId is set
                string truncatedId = quotationId.Substring(0, 8).ToUpper();

                // --- I4/I5: Prepare quotation with rate schedule and discount ---
                _quotationPreparePage.NavigateToPreparePage(quotationId);
                _quotationPreparePage.SelectChargeItem("Walf Booking fee");
                _quotationPreparePage.SelectChargeItem("Lift on/Lif Off");
                Console.WriteLine("[I4] Selected rate schedule items: Walf Booking fee, Lift on/Lif Off");
                _quotationPreparePage.SetDiscount(10);
                Console.WriteLine("[I5] Set discount: 10%");
                _quotationPreparePage.SubmitQuotation();
                Console.WriteLine("[I5] Quotation prepared and submitted by employee.");

                // --- Log out from employee ---
                NavigateToUrl("/Employees/Logout");
                Console.WriteLine("Employee logged out.");

                // --- I6: Login as customer and view quotation ---
                _loginPage.NavigateToLoginPage();
                _loginPage.FillLoginForm(customerEmail, "Test123!");
                _loginPage.SubmitLogin();
                NavigateToUrl("/Users/Dashboard");
                Thread.Sleep(2000); // Wait for the page and table to load

                // Check for the quotation in the dashboard table by truncated Request ID
                var rows = Driver.FindElements(By.CssSelector("table.table tbody tr"));
                bool found = rows.Any(row => (row.FindElements(By.TagName("td")).FirstOrDefault()?.Text ?? "").Contains(truncatedId));
                if (!found)
                {
                    Console.WriteLine($"[ASSERT FAIL] Quotation {quotationId} (Request ID: {truncatedId}) not found in dashboard table after customer login.");
                    Console.WriteLine($"[ASSERT FAIL] Current URL: {Driver.Url}");
                    Console.WriteLine("[ASSERT FAIL] Page source snippet: " + Driver.PageSource.Substring(0, Math.Min(2000, Driver.PageSource.Length)));
                }
                Assert.True(found, $"Quotation with ID {truncatedId} not found in dashboard table after customer login.");
                Console.WriteLine($"[I6] Customer sees quotation: {quotationId}");

                // --- I6: Customer accepts quotation ---
                // Open the modal for the approved quotation using the correct data-bs-target selector
                var viewDetailsButton = Driver.FindElement(By.CssSelector($"button[data-bs-target='#quotationModal-{quotationId}']"));
                ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", viewDetailsButton);
                Thread.Sleep(500);
                viewDetailsButton.Click();

                // Wait for the modal to be visible
                wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
                wait.Until(d => d.FindElement(By.Id($"quotationModal-{quotationId}")).Displayed);
                Thread.Sleep(1000); // Wait for modal animation

                // Click the Accept button in the modal
                var acceptButton = Driver.FindElement(By.CssSelector($"#quotationModal-{quotationId} button[type='submit'][name='CustomerResponseStatus'][value='Accepted']"));
                ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", acceptButton);
                Thread.Sleep(500);
                acceptButton.Click();
                Thread.Sleep(1000); // Wait for response

                // Re-open the modal to check for the response
                viewDetailsButton = Driver.FindElement(By.CssSelector($"button[data-bs-target='#quotationModal-{quotationId}']"));
                viewDetailsButton.Click();
                Thread.Sleep(1000); // Wait for modal to re-open

                // Verify the response is recorded as Accepted
                var modalElement = Driver.FindElement(By.Id($"quotationModal-{quotationId}"));
                var responseInfo = modalElement.FindElement(By.CssSelector(".alert-info")).Text;
                Console.WriteLine($"[DEBUG] Modal .alert-info text after acceptance: '{responseInfo}'");
                if (!responseInfo.Contains("Accepted"))
                {
                    Console.WriteLine("[DEBUG] Full modal HTML after acceptance: " + modalElement.GetAttribute("outerHTML"));
                }
                Assert.Contains("Accepted", responseInfo);
                Console.WriteLine($"[I6] Customer accepted quotation: {quotationId}");

                // --- Log out from customer ---
                NavigateToUrl("/Users/Logout");
                Console.WriteLine("Customer logged out.");

                // --- I5: Login as employee and check quotation status ---
                _employeeLoginPage.NavigateToLoginPage();
                _employeeLoginPage.FillLoginForm(employeeEmail, "Test123!");
                _employeeLoginPage.SubmitLogin();
                _quotationConfirmPage.NavigateToConfirmPage();
                Thread.Sleep(2000); // Wait for table to load

                // Find the row with the correct Request ID (first 8 chars, uppercase)
                var confirmRows = Driver.FindElements(By.CssSelector("table.table tbody tr"));
                IWebElement targetRow = null;
                foreach (var row in confirmRows)
                {
                    var firstCell = row.FindElements(By.TagName("td")).FirstOrDefault();
                    if (firstCell != null && firstCell.Text.Contains(truncatedId))
                    {
                        targetRow = row;
                        break;
                    }
                }
                Assert.NotNull(targetRow);

                // Click the 'View Details' button in that row
                var viewDetailsBtn = targetRow.FindElement(By.CssSelector("a.btn-info.btn-sm"));
                viewDetailsBtn.Click();
                Thread.Sleep(1000); // Wait for modal

                // Check if the modal's .alert-info under 'Status Message' contains 'approved'
                // The modal is rendered as <div class="modal show d-block"> ...
                var modal = Driver.FindElement(By.CssSelector(".modal.show.d-block"));
                var statusMessageInfo = modal.FindElements(By.CssSelector(".alert-info")).LastOrDefault();
                var statusMessageText = statusMessageInfo?.Text.ToLower() ?? string.Empty;
                Console.WriteLine($"[DEBUG] Employee modal status message .alert-info: '{statusMessageText}'");
                Assert.Contains("approved", statusMessageText);
                Console.WriteLine($"[I5] Employee sees quotation status message: {statusMessageText}");
            }
            catch (Exception ex)
            {
                Assert.True(false, $"Test failed: {ex.Message}");
            }
        }
    }
}