using Domain.Entities;

namespace Application.Database;

public interface IUnitOfWork : IDisposable
{
    IRepository<Payment> Payments { get; }

    IRepository<PaymentDetail> PaymentDetails { get; }

    Task SaveAsync();
}
