using InterCargo.BusinessLogic.Entities;

namespace InterCargo.BusinessLogic.Interfaces
{
    public interface IQuotationService
    {
        Task AddQuotationAsync(Quotation quotation);
        Task<List<Quotation>> GetQuotationsByCustomerAsync(string customerId);
        Task<Quotation?> GetQuotationByIdAsync(Guid id);
    }
}
