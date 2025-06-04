using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InterCargo.Application.Interfaces;
using InterCargo.BusinessLogic.Entities;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace InterCargo.Pages.Quotations
{
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

        public async Task<IActionResult> OnGetAsync(string status = null, Guid? viewId = null, bool showRejectForm = false)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { returnUrl = Url.Page("/Quotations/Confirm") });
            }

            try
            {
                var allQuotations = await _quotationService.GetPendingQuotationsAsync();
                AllQuotations = await _quotationService.GetAllQuotationsAsync();

                if (!string.IsNullOrEmpty(status) && status.ToLower() != "all")
                {
                    AllQuotations = AllQuotations.Where(q => q.Status.ToLower() == status.ToLower()).ToList();
                }
                FilterStatus = status;
                PriceBreakdowns = AllQuotations.ToDictionary(
                    q => q.Id,
                    q => _quotationService.GetRateBreakdown(q.ContainerType, q.NumberOfContainers)
                );
                if (viewId.HasValue)
                {
                    SelectedQuotation = AllQuotations.FirstOrDefault(q => q.Id == viewId.Value);
                    if (SelectedQuotation != null)
                    {
                        SelectedUser = _userAppService.GetUserById(SelectedQuotation.CustomerId);
                    }
                }
                ShowRejectForm = showRejectForm;
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching quotations");
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostAsync(Guid quotationId, string action, string rejectionMessage = null)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { returnUrl = Url.Page("/Quotations/Confirm") });
            }

            try
            {
                var quotation = await _quotationService.GetQuotationByIdAsync(quotationId);
                if (quotation == null)
                {
                    return NotFound();
                }

                if (action.ToLower() == "reject" && string.IsNullOrWhiteSpace(rejectionMessage))
                {
                    ModelState.AddModelError(string.Empty, "Please provide a reason for rejecting the quotation.");
                    AllQuotations = await _quotationService.GetPendingQuotationsAsync();
                    return Page();
                }

                quotation.Status = action.ToLower() == "approve" ? "Approved" : "Rejected";
                quotation.Message = action.ToLower() == "approve"
                    ? "Quotation approved by employee"
                    : $"Quotation rejected: {rejectionMessage}";

                await _quotationService.UpdateQuotationAsync(quotation);

                TempData["StatusMessage"] = $"Quotation has been {quotation.Status.ToLower()} successfully.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating quotation status");
                return RedirectToPage("/Error");
            }
        }
    }
}