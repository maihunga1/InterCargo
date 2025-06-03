using InterCargo.BusinessLogic.Entities;

namespace InterCargo.Application.Interfaces
{
    public interface IQuotationAppService
    {
        Task<List<Quotation>> GetAllQuotationsAsync();
        Task AddQuotationAsync(Quotation quotation);
        Task<List<Quotation>> GetQuotationsByCustomerAsync(string customerId);
        Task<Quotation?> GetQuotationByIdAsync(Guid id);

        Task<List<Quotation>> GetPendingQuotationsAsync();
        Task UpdateQuotationAsync(Quotation quotation);

        decimal CalculateQuotationPrice(string containerType, int numberOfContainers);

        Dictionary<string, decimal> GetRateBreakdown(string containerType, int numberOfContainers);

        decimal CalculateFinalPrice(string containerType, int numberOfContainers, decimal? discount);
    }
}