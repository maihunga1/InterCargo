using InterCargo.BusinessLogic.Entities;
using InterCargo.BusinessLogic.Interfaces;
using InterCargo.Application.Interfaces;

namespace InterCargo.Application.Services;

public class QuotationAppService : IQuotationAppService
{
    private readonly IQuotationService _quotationService;

    public async Task<List<Quotation>> GetAllQuotationsAsync()
    {
        return await _quotationService.GetAllQuotationsAsync();
    }

    public QuotationAppService(IQuotationService quotationService)
    {
        _quotationService = quotationService;
    }

    public async Task AddQuotationAsync(Quotation quotation)
    {
        await _quotationService.AddQuotationAsync(quotation);
    }

    public async Task<List<Quotation>> GetQuotationsByCustomerAsync(string customerId)
    {
        return await _quotationService.GetQuotationsByCustomerAsync(customerId);
    }

    public async Task<Quotation?> GetQuotationByIdAsync(Guid id)
    {
        return await _quotationService.GetQuotationByIdAsync(id);
    }

    public async Task<List<Quotation>> GetPendingQuotationsAsync()
    {
        return await _quotationService.GetPendingQuotationsAsync();
    }

    public async Task UpdateQuotationAsync(Quotation quotation)
    {
        await _quotationService.UpdateQuotationAsync(quotation);
    }

    public decimal CalculateQuotationPrice(string containerType, int numberOfContainers)
    {
        return _quotationService.CalculateQuotationPrice(containerType, numberOfContainers);
    }

    public Dictionary<string, decimal> GetRateBreakdown(string containerType, int numberOfContainers)
    {
        return _quotationService.GetRateBreakdown(containerType, numberOfContainers);
    }

    public decimal CalculateFinalPrice(string containerType, int numberOfContainers, decimal? discount)
    {
        return _quotationService.CalculateFinalPrice(containerType, numberOfContainers, discount);
    }
}