using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Database;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
        : base(options) { }

    public DbSet<Payment> PaymentTransactions { get; set; }

    public DbSet<PaymentDetail> TransactionEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<Payment>()
        //    .HasMany(t => t.Events)
        //    .WithOne(e => e.PaymentTransaction)
        //    .HasForeignKey(e => e.PaymentTransactionId);
    }
}
