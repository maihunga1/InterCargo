using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using InterCargo.Application.Interfaces;
using InterCargo.BusinessLogic.Entities;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace InterCargo.Pages.Quotations
{
    [Authorize(Policy = "Employee")]
    public class ConfirmModel : PageModel
    {
        private readonly IQuotationAppService _quotationService;
        private readonly ILogger<ConfirmModel> _logger;
        private readonly IUserAppService _userAppService;

        public ConfirmModel(IQuotationAppService quotationService, ILogger<ConfirmModel> logger, IUserAppService userAppService)
        {
            _quotationService = quotationService;
            _logger = logger;
            _userAppService = userAppService;
        }

        public List<Quotation> AllQuotations { get; set; } = new List<Quotation>();
        public Dictionary<Guid, Dictionary<string, decimal>> PriceBreakdowns { get; set; } = new();
        public string FilterStatus { get; set; }
        public Quotation SelectedQuotation { get; set; }
        public User SelectedUser { get; set; }
        public bool ShowRejectForm { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? viewId = null, string status = "all")
        {
            // Check if user is authenticated and is an employee
            if (!User.Identity.IsAuthenticated || !User.HasClaim("UserRole", "Employee"))
            {
                return RedirectToPage("/Employees/LoginEmployee");
            }

            FilterStatus = status;

            try
            {
                // Get all quotations
                AllQuotations = await _quotationService.GetAllQuotationsAsync();

                // Apply status filter
                if (!string.IsNullOrEmpty(status) && status != "all")
                {
                    AllQuotations = AllQuotations.Where(q => q.Status.ToLower() == status.ToLower()).ToList();
                }

                // Sort by date, newest first
                AllQuotations = AllQuotations.OrderByDescending(q => q.DateIssued).ToList();

                if (viewId.HasValue)
                {
                    SelectedQuotation = AllQuotations.FirstOrDefault(q => q.Id == viewId);
                    if (SelectedQuotation != null)
                    {
                        SelectedUser = await _userAppService.GetUserById(SelectedQuotation.CustomerId);
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading quotations");
                TempData["ErrorMessage"] = "An error occurred while loading quotations. Please try again.";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync(Guid quotationId, string action, string rejectionMessage)
        {
            if (!User.Identity.IsAuthenticated || !User.HasClaim("UserRole", "Employee"))
            {
                return RedirectToPage("/Employees/LoginEmployee");
            }

            try
            {
                var quotation = await _quotationService.GetQuotationByIdAsync(quotationId);
                if (quotation == null)
                {
                    return NotFound();
                }

                if (action == "reject")
                {
                    quotation.Status = "Rejected";
                    quotation.Message = $"Quotation rejected by staff: {rejectionMessage}";
                    await _quotationService.UpdateQuotationAsync(quotation);
                    TempData["StatusMessage"] = "Quotation has been rejected.";
                }

                return RedirectToPage(new { status = FilterStatus });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing quotation");
                TempData["ErrorMessage"] = "An error occurred while processing the quotation. Please try again.";
                return RedirectToPage(new { status = FilterStatus });
            }
        }
    }
}