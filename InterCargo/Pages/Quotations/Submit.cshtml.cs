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
        private readonly ILogger<SubmitModel> _logger;
        private static readonly HashSet<string> SubmittedRequestIds = new HashSet<string>();

        public SubmitModel(IQuotationAppService quotationService, ILogger<SubmitModel> logger)
        {
            _quotationService = quotationService;
            _logger = logger;
        }

        [BindProperty]
        public SubmitInputModel Input { get; set; } = new SubmitInputModel();

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public string RequestId { get; set; }

        public Dictionary<string, decimal> PriceBreakdown { get; set; } = new();

        public IActionResult OnGet()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Users/LoginUser", new { returnUrl = Url.Page("/Quotations/Submit") });
            }

            // Clear any existing status message when starting a new quotation
            StatusMessage = null;

            // Only generate new RequestId if it's not already set
            if (string.IsNullOrEmpty(RequestId))
            {
                RequestId = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            }

            if (!string.IsNullOrEmpty(Input.ContainerType) && Input.NumberOfContainers > 0)
            {
                PriceBreakdown = _quotationService.GetRateBreakdown(Input.ContainerType, Input.NumberOfContainers);
            }
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
                if (!string.IsNullOrEmpty(Input.ContainerType) && Input.NumberOfContainers > 0)
                {
                    PriceBreakdown = _quotationService.GetRateBreakdown(Input.ContainerType, Input.NumberOfContainers);
                }
                _logger.LogWarning("Model state is invalid: {ValidationErrors}",
                    string.Join(", ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)));
                return Page();
            }

            // Check if this RequestId has already been submitted
            if (SubmittedRequestIds.Contains(RequestId))
            {
                ModelState.AddModelError(string.Empty, "This quotation has already been submitted.");
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

                var quotation = new Quotation
                {
                    Id = Guid.NewGuid(),
                    CustomerId = Guid.Parse(userId),
                    Source = Input.Source,
                    Destination = Input.Destination,
                    NumberOfContainers = Input.NumberOfContainers,
                    PackageNature = Input.PackageNature,
                    ImportExportType = Input.ImportExportType,
                    PackingUnpacking = Input.PackingUnpackingType,
                    QuarantineRequirements = Input.QuarantineRequirements,
                    Status = "Pending",
                    Message = $"New quotation request from customer (Request ID: {RequestId})",
                    CustomerResponseStatus = "Pending",
                    DateIssued = DateTime.UtcNow,
                    ContainerType = Input.ContainerType,
                    RequestId = RequestId
                };

                await _quotationService.AddQuotationAsync(quotation);
                
                // Add the RequestId to the set of submitted IDs
                SubmittedRequestIds.Add(RequestId);
                
                _logger.LogInformation("Quotation saved successfully with status: {Status}", quotation.Status);

                StatusMessage = "Quotation submitted successfully! You will be redirected to your dashboard in 2 seconds...";
                TempData["SubmittedRequestId"] = RequestId;
                return Page();  // Return to the same page to show the success message and trigger the redirect
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while submitting quotation");
                ModelState.AddModelError(string.Empty, "An error occurred while submitting the quotation. Please try again.");
                return Page();
            }
        }

        public class SubmitInputModel
        {
            [Required(ErrorMessage = "Source is required")]
            public string Source { get; set; }

            [Required(ErrorMessage = "Destination is required")]
            public string Destination { get; set; }

            [Required(ErrorMessage = "Number of containers is required")]
            [Range(1, int.MaxValue, ErrorMessage = "Number of containers must be at least 1")]
            public int NumberOfContainers { get; set; }

            [Required(ErrorMessage = "Package nature is required")]
            public string PackageNature { get; set; }

            [Required(ErrorMessage = "Import/Export type is required")]
            [Display(Name = "Import/Export Type")]
            public string ImportExportType { get; set; }

            [Required(ErrorMessage = "Packing/Unpacking requirement is required")]
            [Display(Name = "Packing/Unpacking Type")]
            public string PackingUnpackingType { get; set; }

            [Required(ErrorMessage = "Quarantine requirements are required")]
            [Display(Name = "Quarantine Requirements")]
            public string QuarantineRequirements { get; set; }

            [Required(ErrorMessage = "Container type is required")]
            public string ContainerType { get; set; }
        }
    }
}
