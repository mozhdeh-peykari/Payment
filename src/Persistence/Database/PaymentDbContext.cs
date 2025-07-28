using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityTypeConfigurations;

namespace Persistence.Database;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
        : base(options) { }

    public DbSet<Payment> Payments { get; set; }

    public DbSet<PaymentDetail> PaymentDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentTypeConfiguration).Assembly);
    }
}
