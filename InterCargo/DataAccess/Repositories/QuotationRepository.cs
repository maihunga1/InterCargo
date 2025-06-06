using InterCargo.BusinessLogic.Entities;
using InterCargo.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InterCargo.DataAccess.Repositories;

public class QuotationRepository : IQuotationRepository
{
    private readonly ApplicationDbContext _context;

    public QuotationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Quotation>> GetAllQuotationsAsync()
    {
        return await _context.Quotations.ToListAsync();
    }

    public async Task AddQuotationAsync(Quotation quotation)
    {
        _context.Quotations.Add(quotation);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Quotation>> GetQuotationsByCustomerAsync(string customerId)
    {
        return await _context.Quotations
            .Where(q => q.CustomerId.ToString() == customerId)
            .ToListAsync();
    }

    public async Task<Quotation?> GetQuotationByIdAsync(Guid id)
    {
        return await _context.Quotations.FindAsync(id);
    }

    public async Task<List<Quotation>> GetPendingQuotationsAsync()
    {
        return await _context.Quotations
            .Where(q => q.Status == "Pending")
            .ToListAsync();
    }

    public async Task UpdateQuotationAsync(Quotation quotation)
    {
        _context.Quotations.Update(quotation);
        await _context.SaveChangesAsync();
    }
}

