using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InterCargo.Application.Interfaces;
using InterCargo.BusinessLogic.Entities;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace InterCargo.Pages.Quotations
{
    public class SubmitModel : PageModel
    {
        private readonly IQuotationAppService _quotationService;

        public SubmitModel(IQuotationAppService quotationService)
        {
            _quotationService = quotationService;
        }

        [BindProperty]
        public Quotation Quotation { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public IActionResult OnGet()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { returnUrl = Url.Page("/Quotations/Submit") });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { returnUrl = Url.Page("/Quotations/Submit") });
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    ModelState.AddModelError(string.Empty, "User ID not found. Please try logging in again.");
                    return Page();
                }

                Quotation.CustomerId = Guid.Parse(userId);
                Quotation.DateIssued = DateTime.UtcNow;
                Quotation.Status = "Pending";

                await _quotationService.AddQuotationAsync(Quotation);

                StatusMessage = "Quotation submitted successfully!";
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while submitting the quotation. Please try again.");
                return Page();
            }
        }
    }
}
