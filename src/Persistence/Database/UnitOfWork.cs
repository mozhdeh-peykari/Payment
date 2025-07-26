using Domain.Entities;
using Domain.Interfaces;

namespace Persistence.Database
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PaymentDbContext _context;
        private IRepository<Payment>? _payments;
        private IRepository<PaymentDetail>? _paymentDetails;

        public UnitOfWork(PaymentDbContext context)
        {
            _context = context;
        }

        public IRepository<Payment> Payments => _payments ??= new Repository<Payment>(_context);
        public IRepository<PaymentDetail> PaymentDetails => _paymentDetails ??= new Repository<PaymentDetail>(_context);

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
