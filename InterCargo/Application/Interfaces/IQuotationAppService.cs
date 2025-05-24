using InterCargo.BusinessLogic.Entities;

namespace InterCargo.Application.Interfaces
{
    public interface IQuotationAppService
    {
        Task AddQuotationAsync(Quotation quotation);
        Task<List<Quotation>> GetQuotationsByCustomerAsync(string customerId);
        Task<Quotation?> GetQuotationByIdAsync(int id);
    }
}