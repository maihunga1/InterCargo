using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Web;
using System.Threading;
using System.IO;
using System.Linq;

namespace InterCargo.UITests.Pages
{
    public class QuotationConfirmPage : BasePage
    {
        public QuotationConfirmPage(IWebDriver driver) : base(driver)
        {
        }

        // Locators
        private By StatusFilter(string status) => By.CssSelector($"a[href*='status={status}']");
        private By ViewDetailsButton(string quotationId) => By.CssSelector($"a[href*='viewId={quotationId}']");
        private By RejectButton => By.CssSelector("button[onclick*='showRejectPopup']");
        private By RejectModal => By.Id("rejectModalPopup");
        private By RejectionMessageInput => By.Id("rejectionMessagePopup");
        private By ConfirmRejectionButton => By.CssSelector("button[name='action'][value='reject']");
        private By SuccessMessage => By.CssSelector(".alert-success");
        private By ErrorMessage => By.CssSelector(".alert-danger, .validation-summary-errors, .text-danger");
        private By QuotationTable => By.CssSelector("table.table");
        private By QuotationRows => By.CssSelector("table.table tbody tr");
        private By QuotationStatus(string quotationId) => By.CssSelector($"tr[data-quotation-id='{quotationId}'] .badge");

        public void NavigateToConfirmPage()
        {
            NavigateToUrl("/Quotations/Confirm");
        }

        public void FilterByStatus(string status)
        {
            try
            {
                Click(StatusFilter(status));
            }
            catch
            {
                // Log error or handle gracefully
            }
        }

        public void ViewQuotationDetails(string quotationId)
        {
            try
            {
                Click(ViewDetailsButton(quotationId));
            }
            catch
            {
                // Log error or handle gracefully
            }
        }

        public void RejectQuotation(string rejectionMessage)
        {
            try
            {
                Click(RejectButton);
                WaitForElement(RejectModal);
                SendKeys(RejectionMessageInput, rejectionMessage);
                Click(ConfirmRejectionButton);
            }
            catch
            {
                // Log error or handle gracefully
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

        public int GetQuotationCount()
        {
            try
            {
                return Driver.FindElements(QuotationRows).Count;
            }
            catch
            {
                return 0;
            }
        }

        public bool IsQuotationInTable(string quotationId)
        {
            try
            {
                // Truncate the quotation ID to 8 characters to match the dashboard display
                string truncatedId = quotationId.Substring(0, 8).ToUpper();
                Console.WriteLine($"[DEBUG] Looking for Request ID: {truncatedId}");

                // Wait for table to be visible
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
                wait.Until(d => d.FindElement(QuotationTable).Displayed);

                // Get all Request IDs in the table
                var rows = Driver.FindElements(By.CssSelector("table.table tbody tr"));
                var requestIds = rows.Select(row => row.FindElements(By.TagName("td")).FirstOrDefault()?.Text ?? "").ToList();
                Console.WriteLine("[DEBUG] Request IDs in table: " + string.Join(", ", requestIds));

                if (requestIds.Count == 0)
                {
                    Console.WriteLine($"[DEBUG] Current URL: {Driver.Url}");
                    Console.WriteLine("[DEBUG] Page source snippet: " + Driver.PageSource.Substring(0, Math.Min(2000, Driver.PageSource.Length)));
                }

                // Wait for the specific quotation to appear
                return wait.Until(d =>
                {
                    try
                    {
                        var currentRows = d.FindElements(By.CssSelector("table.table tbody tr"));
                        return currentRows.Any(row => (row.FindElements(By.TagName("td")).FirstOrDefault()?.Text ?? "").Contains(truncatedId));
                    }
                    catch
                    {
                        return false;
                    }
                });
            }
            catch
            {
                return false;
            }
        }

        public string GetQuotationStatus(string quotationId)
        {
            try
            {
                return GetText(QuotationStatus(quotationId));
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GetLatestQuotationId()
        {
            try
            {
                var rows = Driver.FindElements(QuotationRows);
                if (rows.Count == 0) return null;
                var firstRow = rows[0];
                // Find the 'View Details' button in the first row
                var viewDetailsButton = firstRow.FindElement(By.CssSelector("a.btn-info"));
                var href = viewDetailsButton.GetAttribute("href");
                // Extract the viewId parameter from the href
                var uri = new Uri(href, UriKind.RelativeOrAbsolute);
                var query = uri.Query;
                var viewIdParam = System.Web.HttpUtility.ParseQueryString(query)["viewId"];
                return viewIdParam;
            }
            catch
            {
                return null;
            }
        }

        public void AcceptQuotation(string quotationId)
        {
            try
            {
                // Maximize window to ensure modal is fully visible
                Driver.Manage().Window.Maximize();
                Thread.Sleep(1000); // Wait for maximize to complete

                // Use the full, lowercase GUID for the selector
                string guidForSelector = quotationId.ToLower();

                // First click the View Details button to open the modal
                var viewDetailsButton = Driver.FindElement(By.CssSelector($"a.btn-info.btn-sm[href*='viewId={guidForSelector}']"));
                ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", viewDetailsButton);
                Thread.Sleep(500);
                viewDetailsButton.Click();

                // Wait for modal to be visible and fully loaded
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
                wait.Until(d => d.FindElement(By.Id("quotationDetailsModal")).Displayed);

                // Wait for modal animation to complete
                Thread.Sleep(2000);

                // Print modal style info
                try
                {
                    var modal = Driver.FindElement(By.Id("quotationDetailsModal"));
                    var zIndex = ((IJavaScriptExecutor)Driver).ExecuteScript("return window.getComputedStyle(arguments[0]).zIndex;", modal);
                    var display = ((IJavaScriptExecutor)Driver).ExecuteScript("return window.getComputedStyle(arguments[0]).display;", modal);
                    var visibility = ((IJavaScriptExecutor)Driver).ExecuteScript("return window.getComputedStyle(arguments[0]).visibility;", modal);
                    Console.WriteLine($"Modal z-index: {zIndex}, display: {display}, visibility: {visibility}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting modal style info: {ex.Message}");
                }

                // Take screenshot before clicking
                try
                {
                    var screenshot = ((ITakesScreenshot)Driver).GetScreenshot();
                    var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"AcceptQuotation_BeforeClick_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                    screenshot.SaveAsFile(filePath);
                    Console.WriteLine($"Screenshot saved: {filePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error taking screenshot: {ex.Message}");
                }

                // Find the Accept link in the modal footer
                var acceptLink = wait.Until(d =>
                {
                    try
                    {
                        var link = d.FindElement(By.CssSelector("#quotationDetailsModal .modal-footer a.btn-success.btn-sm[href*='/Quotations/Prepare']"));
                        ((IJavaScriptExecutor)d).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", link);
                        Thread.Sleep(500);
                        return link.Displayed && link.Enabled ? link : null;
                    }
                    catch
                    {
                        return null;
                    }
                });

                if (acceptLink == null)
                {
                    throw new Exception("Accept link not found in modal footer");
                }

                // Try to click the Accept link
                try
                {
                    var actions = new OpenQA.Selenium.Interactions.Actions(Driver);
                    actions.MoveToElement(acceptLink).Click().Perform();
                }
                catch
                {
                    try
                    {
                        acceptLink.Click();
                    }
                    catch
                    {
                        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", acceptLink);
                    }
                }

                // Wait for navigation to the Prepare page
                wait.Until(d => d.Url.Contains("/Quotations/Prepare"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accepting quotation: {ex.Message}");
                throw;
            }
        }
    }
}