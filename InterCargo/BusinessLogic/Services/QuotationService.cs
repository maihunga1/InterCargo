using InterCargo.BusinessLogic.Entities;
using InterCargo.BusinessLogic.Interfaces;
using InterCargo.DataAccess.Interfaces;

namespace InterCargo.BusinessLogic.Services;

public class QuotationService : IQuotationService
{
    private readonly IQuotationRepository _quotationRepository;

    public async Task<List<Quotation>> GetAllQuotationsAsync()
    {
        return await _quotationRepository.GetAllQuotationsAsync();
    }

    public QuotationService(IQuotationRepository quotationRepository)
    {
        _quotationRepository = quotationRepository;
    }

    public async Task AddQuotationAsync(Quotation quotation)
    {
        await _quotationRepository.AddQuotationAsync(quotation);
    }

    public async Task<List<Quotation>> GetQuotationsByCustomerAsync(string customerId)
    {
        return await _quotationRepository.GetQuotationsByCustomerAsync(customerId);
    }

    public async Task<Quotation?> GetQuotationByIdAsync(Guid id)
    {
        return await _quotationRepository.GetQuotationByIdAsync(id);
    }

    public async Task<List<Quotation>> GetPendingQuotationsAsync()
    {
        return await _quotationRepository.GetPendingQuotationsAsync();
    }

    public async Task UpdateQuotationAsync(Quotation quotation)
    {
        await _quotationRepository.UpdateQuotationAsync(quotation);
    }

    public decimal CalculateQuotationPrice(string containerType, int numberOfContainers)
    {
        // Rate schedule
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
        decimal subtotal = 0;
        if (containerType == "20Feet")
        {
            subtotal = rates20.Values.Sum() * numberOfContainers;
        }
        else if (containerType == "40Feet")
        {
            subtotal = rates40.Values.Sum() * numberOfContainers;
        }
        else
        {
            throw new ArgumentException("Invalid container type");
        }
        decimal gst = subtotal * 0.10m;
        return subtotal + gst;
    }

    public Dictionary<string, decimal> GetRateBreakdown(string containerType, int numberOfContainers)
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
        var breakdown = new Dictionary<string, decimal>();
        var rates = containerType == "20Feet" ? rates20 : containerType == "40Feet" ? rates40 : null;
        if (rates == null) throw new ArgumentException("Invalid container type");
        foreach (var item in rates)
        {
            breakdown[item.Key] = item.Value * numberOfContainers;
        }
        breakdown["GST (10%)"] = rates.Values.Sum() * numberOfContainers * 0.10m;
        breakdown["Total"] = rates.Values.Sum() * numberOfContainers * 1.10m;
        return breakdown;
    }

    public decimal CalculateFinalPrice(string containerType, int numberOfContainers, decimal? discount)
    {
        var breakdown = GetRateBreakdown(containerType, numberOfContainers);
        var total = breakdown.ContainsKey("Total") ? breakdown["Total"] : breakdown.Values.Sum();
        if (discount.HasValue && discount.Value > 0)
        {
            total -= discount.Value;
            if (total < 0) total = 0;
        }
        return total;
    }
}