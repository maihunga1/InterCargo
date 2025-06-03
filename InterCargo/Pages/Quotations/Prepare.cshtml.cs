using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InterCargo.Application.Interfaces;
using InterCargo.BusinessLogic.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace InterCargo.Pages.Quotations
{
    public class PrepareModel : PageModel
    {
        private readonly IQuotationAppService _quotationService;
        private readonly IUserAppService _userAppService;

        public PrepareModel(IQuotationAppService quotationService, IUserAppService userAppService)
        {
            _quotationService = quotationService;
            _userAppService = userAppService;
        }

        [BindProperty]
        public PrepareInputModel Input { get; set; } = new PrepareInputModel();
        public List<User> Customers { get; set; } = new();
        public Dictionary<string, decimal> PriceBreakdown { get; set; } = new();
        public decimal FinalPrice { get; set; }
        public string StatusMessage { get; set; }
        public List<string> AllChargeItems { get; set; } = new List<string> {
            "Walf Booking fee",
            "Lift on/Lif Off",
            "Fumigation",
            "LCL Delivery Depot",
            "Tailgate Inspection",
            "Storafe Fee",
            "Facility Fee",
            "Walf Inspection"
        };
        [BindProperty]
        public List<string> SelectedChargeItems { get; set; } = new();
        public Quotation SelectedQuotation { get; set; }
        public User SelectedUser { get; set; }

        public void OnGet(Guid? quotationId = null)
        {
            Customers = _userAppService.GetAllUsers();
            if (quotationId.HasValue)
            {
                SelectedQuotation = _quotationService.GetQuotationByIdAsync(quotationId.Value).Result;
                if (SelectedQuotation != null)
                {
                    SelectedUser = _userAppService.GetUserById(SelectedQuotation.CustomerId);
                    // Pre-fill form fields
                    Input = new PrepareInputModel
                    {
                        CustomerId = SelectedQuotation.CustomerId.ToString(),
                        Source = SelectedQuotation.Source,
                        Destination = SelectedQuotation.Destination,
                        NumberOfContainers = SelectedQuotation.NumberOfContainers,
                        PackageNature = SelectedQuotation.PackageNature,
                        ImportExportType = SelectedQuotation.ImportExportType,
                        RequiresPacking = SelectedQuotation.PackingUnpacking.Contains("Packing: Required"),
                        RequiresUnpacking = SelectedQuotation.PackingUnpacking.Contains("Unpacking: Required"),
                        QuarantineRequirements = SelectedQuotation.QuarantineRequirements,
                        ContainerType = SelectedQuotation.ContainerType,
                        Discount = SelectedQuotation.Discount
                    };
                    if (!string.IsNullOrEmpty(SelectedQuotation.SelectedChargeItemsJson))
                    {
                        SelectedChargeItems = JsonSerializer.Deserialize<List<string>>(SelectedQuotation.SelectedChargeItemsJson) ?? new List<string>();
                    }
                }
            }
            if (SelectedChargeItems == null || SelectedChargeItems.Count == 0)
            {
                SelectedChargeItems = new List<string>(AllChargeItems);
            }
            if (!string.IsNullOrEmpty(Input.ContainerType) && Input.NumberOfContainers > 0)
            {
                PriceBreakdown = GetCustomRateBreakdown(Input.ContainerType, Input.NumberOfContainers, SelectedChargeItems);
                FinalPrice = CalculateFinalPriceWithPercentage(Input.ContainerType, Input.NumberOfContainers, SelectedChargeItems, Input.Discount);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Customers = _userAppService.GetAllUsers();
            if (!ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(Input.ContainerType) && Input.NumberOfContainers > 0)
                {
                    PriceBreakdown = GetCustomRateBreakdown(Input.ContainerType, Input.NumberOfContainers, SelectedChargeItems);
                    FinalPrice = CalculateFinalPriceWithPercentage(Input.ContainerType, Input.NumberOfContainers, SelectedChargeItems, Input.Discount);
                }
                return Page();
            }

            var quotation = new Quotation
            {
                Id = Guid.NewGuid(),
                CustomerId = Guid.Parse(Input.CustomerId),
                Source = Input.Source,
                Destination = Input.Destination,
                NumberOfContainers = Input.NumberOfContainers,
                PackageNature = Input.PackageNature,
                ImportExportType = Input.ImportExportType,
                PackingUnpacking = $"Packing: {(Input.RequiresPacking ? "Required" : "Not Required")}, " +
                                 $"Unpacking: {(Input.RequiresUnpacking ? "Required" : "Not Required")}",
                QuarantineRequirements = Input.QuarantineRequirements,
                Status = "Approved",
                Message = "Quotation approved and prepared by employee",
                DateIssued = DateTime.UtcNow,
                ContainerType = Input.ContainerType,
                Discount = Input.Discount,
                FinalPrice = CalculateFinalPriceWithPercentage(Input.ContainerType, Input.NumberOfContainers, SelectedChargeItems, Input.Discount),
                SelectedChargeItemsJson = JsonSerializer.Serialize(SelectedChargeItems)
            };

            await _quotationService.AddQuotationAsync(quotation);
            StatusMessage = "Quotation submitted successfully!";
            return RedirectToPage("/Quotations/Confirm");
        }

        public Dictionary<string, decimal> GetCustomRateBreakdown(string containerType, int numberOfContainers, List<string> selectedItems)
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

        public decimal CalculateFinalPriceWithPercentage(string containerType, int numberOfContainers, List<string> selectedItems, decimal? discountPercent)
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

        public class PrepareInputModel
        {
            [Required]
            public string CustomerId { get; set; }
            [Required]
            public string Source { get; set; }
            [Required]
            public string Destination { get; set; }
            [Required]
            [Range(1, int.MaxValue)]
            public int NumberOfContainers { get; set; }
            [Required]
            public string PackageNature { get; set; }
            [Required]
            public string ImportExportType { get; set; }
            public bool RequiresPacking { get; set; }
            public bool RequiresUnpacking { get; set; }
            [Required]
            public string QuarantineRequirements { get; set; }
            [Required]
            public string ContainerType { get; set; }
            public decimal? Discount { get; set; }
        }
    }
}