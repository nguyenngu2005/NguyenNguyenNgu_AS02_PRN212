using Data.Entities;

namespace Data.Repositories
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IRepository<Customer> Customers { get; }
        IRepository<RoomType> RoomTypes { get; }
        IRepository<RoomInformation> Rooms { get; }
        IRepository<BookingReservation> Reservations { get; }
        IRepository<BookingDetail> BookingDetails { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken ct = default);
    }
}
