using InterCargo.BusinessLogic.Entities;
using InterCargo.BusinessLogic.Interfaces;
using InterCargo.DataAccess.Interfaces;

namespace InterCargo.BusinessLogic.Services;

public class QuotationService : IQuotationService
{
    private readonly IQuotationRepository _quotationRepository;

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
}