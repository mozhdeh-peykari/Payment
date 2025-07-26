using Domain.Entities;
using Domain.Interfaces;

namespace Persistence.Database
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PaymentDbContext _context;
        private IRepository<Payment>? _transactions;

        public UnitOfWork(PaymentDbContext context)
        {
            _context = context;
        }

        public IRepository<Payment> Transactions => _transactions ??= new Repository<Payment>(_context);

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
