using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InterCargo.Application.Interfaces;
using InterCargo.BusinessLogic.Entities;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace InterCargo.Pages.Quotations
{
    public class SubmitModel : PageModel
    {
        private readonly IQuotationAppService _quotationService;
        private readonly IUserAppService _userService;
        private readonly ILogger<SubmitModel> _logger;

        public SubmitModel(
            IQuotationAppService quotationService, 
            IUserAppService userService,
            ILogger<SubmitModel> logger)
        {
            _quotationService = quotationService;
            _userService = userService;
            _logger = logger;
        }

        [BindProperty]
        public Quotation Quotation { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerCompany { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Users/LoginUser", new { returnUrl = Url.Page("/Quotations/Submit") });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Users/LoginUser");
            }

            try
            {
                var user = await _userService.GetUserByIdAsync(Guid.Parse(userId));
                if (user != null)
                {
                    CustomerName = $"{user.FirstName} {user.FamilyName}";
                    CustomerEmail = user.Email;
                    CustomerCompany = user.CompanyName;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user information");
                CustomerName = "Not available";
                CustomerEmail = "Not available";
                CustomerCompany = "Not available";
            }

            Quotation = new Quotation
            {
                Id = Guid.NewGuid(),
                CustomerId = Guid.Parse(userId),
                DateIssued = DateTime.UtcNow,
                Status = "Pending",
                Message = "New quotation request"
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Users/LoginUser", new { returnUrl = Url.Page("/Quotations/Submit") });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid: {ValidationErrors}",
                    string.Join(", ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)));

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId))
                {
                    var user = await _userService.GetUserByIdAsync(Guid.Parse(userId));
                    if (user != null)
                    {
                        CustomerName = $"{user.FirstName} {user.FamilyName}";
                        CustomerEmail = user.Email;
                        CustomerCompany = user.CompanyName;
                    }
                }
                return Page();
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _logger.LogInformation("Retrieved user ID: {UserId}", userId);

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogError("User ID not found in claims");
                    ModelState.AddModelError(string.Empty, "User ID not found. Please try logging in again.");
                    return Page();
                }

                Quotation.CustomerId = Guid.Parse(userId);
                Quotation.Status = "Pending";
                Quotation.Message = "New quotation request from customer";
                Quotation.DateIssued = DateTime.UtcNow;

                _logger.LogInformation("Preparing to save quotation: {QuotationDetails}",
                    $"ID: {Quotation.Id}, " +
                    $"CustomerId: {Quotation.CustomerId}, " +
                    $"Source: {Quotation.Source}, " +
                    $"Destination: {Quotation.Destination}, " +
                    $"Containers: {Quotation.NumberOfContainers}, " +
                    $"JobType: {Quotation.JobType}, " +
                    $"RequiresPacking: {Quotation.RequiresPacking}, " +
                    $"RequiresUnpacking: {Quotation.RequiresUnpacking}, " +
                    $"RequiresQuarantine: {Quotation.RequiresQuarantine}, " +
                    $"Status: {Quotation.Status}");

                await _quotationService.AddQuotationAsync(Quotation);
                _logger.LogInformation("Quotation saved successfully with ID: {QuotationId}", Quotation.Id);

                StatusMessage = "Quotation request submitted successfully! Your request ID is: " + Quotation.Id;
                return RedirectToPage("./Confirmation", new { id = Quotation.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while submitting quotation");
                ModelState.AddModelError(string.Empty, "An error occurred while submitting the quotation. Please try again.");
                return Page();
            }
        }

    }
}
