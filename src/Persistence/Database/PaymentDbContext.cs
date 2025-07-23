using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Database;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
        : base(options) { }

    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    public DbSet<TransactionEvent> TransactionEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PaymentTransaction>()
            .HasMany(t => t.Events)
            .WithOne(e => e.PaymentTransaction)
            .HasForeignKey(e => e.PaymentTransactionId);
    }
}
