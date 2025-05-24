using InterCargo.BusinessLogic.Entities;
using InterCargo.BusinessLogic.Interfaces;
using InterCargo.Application.Interfaces;

namespace InterCargo.Application.Services;

public class QuotationAppService : IQuotationAppService
{
    private readonly IQuotationService _quotationService;

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

    public async Task<Quotation?> GetQuotationByIdAsync(int id)
    {
        return await _quotationService.GetQuotationByIdAsync(id);
    }
}