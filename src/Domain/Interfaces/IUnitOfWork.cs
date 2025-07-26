using Domain.Entities;

namespace Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Payment> Payments { get; }

    IRepository<PaymentDetail> PaymentDetails { get; }

    Task SaveAsync();
}
