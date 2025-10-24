using Microsoft.EntityFrameworkCore;
using Data.Entities;

namespace Data
{
    public class FumhContext : DbContext
    {
        public FumhContext(DbContextOptions<FumhContext> options) : base(options) { }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<RoomType> RoomTypes => Set<RoomType>();
        public DbSet<RoomInformation> RoomInformations => Set<RoomInformation>();
        public DbSet<BookingReservation> BookingReservations => Set<BookingReservation>();
        public DbSet<BookingDetail> BookingDetails => Set<BookingDetail>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Customer>(e =>
            {
                e.HasKey(x => x.CustomerID);
                e.HasIndex(x => x.EmailAddress).IsUnique();

                e.Property(x => x.CustomerBirthday).HasColumnType("date");
            });

            mb.Entity<RoomType>(e =>
            {
                e.HasKey(x => x.RoomTypeID);
            });

            mb.Entity<RoomInformation>(e =>
            {
                e.HasKey(x => x.RoomID);

                // MONEY -> decimal(19,4)
                e.Property(x => x.RoomPricePerDay).HasColumnType("decimal(19,4)");

                e.HasOne(x => x.RoomType)
                    .WithMany(rt => rt.Rooms)
                    .HasForeignKey(x => x.RoomTypeID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            mb.Entity<BookingReservation>(e =>
            {
                e.HasKey(x => x.BookingReservationID);

                e.Property(x => x.BookingReservationID).ValueGeneratedNever();

                e.Property(x => x.TotalPrice).HasColumnType("decimal(19,4)");

                e.Property(x => x.BookingDate).HasColumnType("date");

                e.HasOne(x => x.Customer)
                    .WithMany(c => c.BookingReservations)
                    .HasForeignKey(x => x.CustomerID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            mb.Entity<BookingDetail>(e =>
            {
                e.HasKey(x => new { x.BookingReservationID, x.RoomID });

                e.Property(x => x.ActualPrice).HasColumnType("decimal(19,4)");

                e.Property(x => x.StartDate).HasColumnType("date");
                e.Property(x => x.EndDate).HasColumnType("date");

                e.HasOne(x => x.BookingReservation)
                    .WithMany(br => br.BookingDetails)
                    .HasForeignKey(x => x.BookingReservationID)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Room)
                    .WithMany(r => r.BookingDetails)
                    .HasForeignKey(x => x.RoomID)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
