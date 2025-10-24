using Microsoft.EntityFrameworkCore;
using Data.Entities;

namespace Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FumhContext _ctx;

        public IRepository<Customer> Customers { get; }
        public IRepository<RoomType> RoomTypes { get; }
        public IRepository<RoomInformation> Rooms { get; }
        public IRepository<BookingReservation> Reservations { get; }
        public IRepository<BookingDetail> BookingDetails { get; }

        public UnitOfWork(FumhContext ctx)
        {
            _ctx = ctx;
            Customers = new EfRepository<Customer>(_ctx);
            RoomTypes = new EfRepository<RoomType>(_ctx);
            Rooms = new EfRepository<RoomInformation>(_ctx);
            Reservations = new EfRepository<BookingReservation>(_ctx);
            BookingDetails = new EfRepository<BookingDetail>(_ctx);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            _ctx.SaveChangesAsync(cancellationToken);

        public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken ct = default)
        {
            var strategy = _ctx.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var tx = await _ctx.Database.BeginTransactionAsync(ct);
                try
                {
                    await action();
                    await _ctx.SaveChangesAsync(ct);
                    await tx.CommitAsync(ct);
                }
                catch
                {
                    await tx.RollbackAsync(ct);
                    throw;
                }
            });
        }

        public ValueTask DisposeAsync() => _ctx.DisposeAsync();
    }
}
