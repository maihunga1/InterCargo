using InterCargo.BusinessLogic.Entities;

namespace InterCargo.DataAccess.Interfaces
{
    public interface IQuotationRepository
    {
        Task AddQuotationAsync(Quotation quotation);
        Task<List<Quotation>> GetQuotationsByCustomerAsync(string customerId);
        Task<Quotation?> GetQuotationByIdAsync(int id);
    }
}