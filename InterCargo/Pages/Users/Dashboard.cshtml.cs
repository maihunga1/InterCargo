using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InterCargo.Application.Interfaces;
using InterCargo.BusinessLogic.Entities;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace InterCargo.Pages.Users
{
    public class DashboardModel : PageModel
    {
        private readonly IQuotationAppService _quotationService;
        private readonly ILogger<DashboardModel> _logger;
        private readonly IEmployeeAppService _employeeAppService;

        public DashboardModel(IQuotationAppService quotationService, ILogger<DashboardModel> logger, IEmployeeAppService employeeAppService)
        {
            _quotationService = quotationService;
            _logger = logger;
            _employeeAppService = employeeAppService;
        }

        public List<Quotation> UserQuotations { get; set; } = new List<Quotation>();
        public Quotation SelectedQuotation { get; set; }
        [BindProperty]
        public string CustomerResponseMessage { get; set; }
        [BindProperty]
        public string CustomerResponseStatus { get; set; }
        public List<string> SelectedChargeItems { get; set; } = new();
        public Dictionary<string, decimal> PriceBreakdown { get; set; } = new();
        public decimal? FinalPrice { get; set; }
        public Dictionary<Guid, List<string>> QuotationSelectedChargeItems { get; set; } = new();
        public Dictionary<Guid, Dictionary<string, decimal>> QuotationPriceBreakdowns { get; set; } = new();
        public Dictionary<Guid, decimal?> QuotationFinalPrices { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid? viewId = null)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Users/LoginUser", new { returnUrl = Url.Page("/Users/Dashboard") });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Check if this user is an employee
            var isEmployee = _employeeAppService.GetAllEmployees().Any(e => e.Id.ToString() == userId);
            if (isEmployee)
            {
                return RedirectToPage("/Quotations/Confirm");
            }

            try
            {
                UserQuotations = await _quotationService.GetQuotationsByCustomerAsync(userId.ToUpper());

                // If we have a newly submitted quotation, highlight it
                if (TempData["SubmittedRequestId"] is string submittedRequestId)
                {
                    var submittedQuotation = UserQuotations.FirstOrDefault(q => q.RequestId == submittedRequestId);
                    if (submittedQuotation != null)
                    {
                        viewId = submittedQuotation.Id;
                    }
                }

                foreach (var q in UserQuotations)
                {
                    if (!string.IsNullOrEmpty(q.SelectedChargeItemsJson))
                    {
                        var selectedItems = JsonSerializer.Deserialize<List<string>>(q.SelectedChargeItemsJson) ?? new List<string>();
                        QuotationSelectedChargeItems[q.Id] = selectedItems;
                        QuotationPriceBreakdowns[q.Id] = GetCustomRateBreakdown(q.ContainerType, q.NumberOfContainers, selectedItems);
                        QuotationFinalPrices[q.Id] = CalculateFinalPriceWithPercentage(q.ContainerType, q.NumberOfContainers, selectedItems, q.Discount);
                    }
                }
                if (viewId.HasValue)
                {
                    SelectedQuotation = UserQuotations.FirstOrDefault(q => q.Id == viewId.Value);
                    if (SelectedQuotation != null && !string.IsNullOrEmpty(SelectedQuotation.SelectedChargeItemsJson))
                    {
                        SelectedChargeItems = JsonSerializer.Deserialize<List<string>>(SelectedQuotation.SelectedChargeItemsJson) ?? new List<string>();
                        PriceBreakdown = GetCustomRateBreakdown(SelectedQuotation.ContainerType, SelectedQuotation.NumberOfContainers, SelectedChargeItems);
                        FinalPrice = CalculateFinalPriceWithPercentage(SelectedQuotation.ContainerType, SelectedQuotation.NumberOfContainers, SelectedChargeItems, SelectedQuotation.Discount);
                    }
                }
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user quotations");
                return RedirectToPage("/Error");
            }
        }

        private Dictionary<string, decimal> GetCustomRateBreakdown(string containerType, int numberOfContainers, List<string> selectedItems)
        {
            var rates20 = new Dictionary<string, decimal>
            {
                {"Walf Booking fee", 60},
                {"Lift on/Lif Off", 80},
                {"Fumigation", 220},
                {"LCL Delivery Depot", 400},
                {"Tailgate Inspection", 120},
                {"Storafe Fee", 240},
                {"Facility Fee", 70},
                {"Walf Inspection", 60}
            };
            var rates40 = new Dictionary<string, decimal>
            {
                {"Walf Booking fee", 70},
                {"Lift on/Lif Off", 120},
                {"Fumigation", 280},
                {"LCL Delivery Depot", 500},
                {"Tailgate Inspection", 160},
                {"Storafe Fee", 300},
                {"Facility Fee", 100},
                {"Walf Inspection", 90}
            };
            var rates = containerType == "20Feet" ? rates20 : rates40;
            var breakdown = new Dictionary<string, decimal>();
            foreach (var item in selectedItems)
            {
                if (rates.ContainsKey(item))
                    breakdown[item] = rates[item] * numberOfContainers;
            }
            var subtotal = breakdown.Values.Sum();
            breakdown["GST (10%)"] = subtotal * 0.10m;
            breakdown["Total"] = subtotal * 1.10m;
            return breakdown;
        }

        private decimal CalculateFinalPriceWithPercentage(string containerType, int numberOfContainers, List<string> selectedItems, decimal? discountPercent)
        {
            var breakdown = GetCustomRateBreakdown(containerType, numberOfContainers, selectedItems);
            var total = breakdown.ContainsKey("Total") ? breakdown["Total"] : breakdown.Values.Sum();
            if (discountPercent.HasValue && discountPercent.Value > 0)
            {
                var discountAmount = total * (discountPercent.Value / 100m);
                total -= discountAmount;
                if (total < 0) total = 0;
            }
            return total;
        }

        public async Task<IActionResult> OnPostRespondAsync(Guid quotationId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Users/LoginUser", new { returnUrl = Url.Page("/Users/Dashboard") });
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Check if this user is an employee
            var isEmployee = _employeeAppService.GetAllEmployees().Any(e => e.Id.ToString() == userId);
            if (isEmployee)
            {
                return RedirectToPage("/Quotations/Confirm");
            }
            UserQuotations = await _quotationService.GetQuotationsByCustomerAsync(userId.ToUpper());
            SelectedQuotation = UserQuotations.FirstOrDefault(q => q.Id == quotationId);
            if (SelectedQuotation != null)
            {
                SelectedQuotation.CustomerResponseStatus = CustomerResponseStatus;
                SelectedQuotation.CustomerResponseMessage = CustomerResponseMessage;
                if (CustomerResponseStatus == "Declined")
                {
                    SelectedQuotation.Status = "Rejected";
                    SelectedQuotation.Message = "Quotation declined by customer: " + CustomerResponseMessage;
                }
                await _quotationService.UpdateQuotationAsync(SelectedQuotation);
            }
            return RedirectToPage();
        }
    }
}