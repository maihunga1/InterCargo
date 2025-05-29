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
        private readonly IUserAppService _userService;

        public SubmitModel(IQuotationAppService quotationService, IUserAppService userService)
        {
            _quotationService = quotationService;
            _userService = userService;
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
            catch (Exception)
            {
                // Log the error but don't stop the page from loading
                CustomerName = "Not available";
                CustomerEmail = "Not available";
                CustomerCompany = "Not available";
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
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userService.GetUserByIdAsync(Guid.Parse(userId));
                if (user != null)
                {
                    CustomerName = $"{user.FirstName} {user.FamilyName}";
                    CustomerEmail = user.Email;
                    CustomerCompany = user.CompanyName;
                }
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

                StatusMessage = "Quotation request submitted successfully! Your request ID is: " + Quotation.Id;
                return RedirectToPage("./Confirmation", new { id = Quotation.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while submitting the quotation. Please try again.");
                return Page();
            }
        }
    }
}
