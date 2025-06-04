using InterCargo.BusinessLogic.Entities;

namespace InterCargo.DataAccess.Interfaces
{
    public interface IQuotationRepository
    {
        Task<List<Quotation>> GetAllQuotationsAsync();
        Task AddQuotationAsync(Quotation quotation);
        Task<List<Quotation>> GetQuotationsByCustomerAsync(string customerId);
        Task<Quotation?> GetQuotationByIdAsync(Guid id);
        Task<List<Quotation>> GetPendingQuotationsAsync();
        Task UpdateQuotationAsync(Quotation quotation);
    }
}