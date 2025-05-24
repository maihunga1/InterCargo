using Microsoft.EntityFrameworkCore;
using InterCargo.BusinessLogic.Entities;

namespace InterCargo.DataAccess;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Quotation> Quotations { get; set; }
}