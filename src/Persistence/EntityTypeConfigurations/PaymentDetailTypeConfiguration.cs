using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityTypeConfigurations;

public class PaymentDetailTypeConfiguration : IEntityTypeConfiguration<PaymentDetail>
{
    public void Configure(EntityTypeBuilder<PaymentDetail> builder)
    {
        builder
           .HasKey(p => p.Id);

        builder
            .Property(p => p.Request)
            .HasColumnType("nvarchar(max)");

        builder
            .Property(p => p.Response)
            .HasColumnType("nvarchar(max)");
    }
}
