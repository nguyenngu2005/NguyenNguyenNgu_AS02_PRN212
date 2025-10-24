using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Business.Abstractions;
using Business.Dtos;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;


namespace NguyenNguyenNguWPF.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly ICustomerService _customers;
        private readonly IRoomService _rooms;
        private readonly IBookingService _bookings;
        private readonly IReportService _reports;

        [ObservableProperty] private DateTime? fromDate = DateTime.Today.AddDays(-7);
        [ObservableProperty] private DateTime? toDate = DateTime.Today;

        [ObservableProperty] private int totalCustomers;
        [ObservableProperty] private int roomsAvailable;
        [ObservableProperty] private int bookingsToday;
        [ObservableProperty] private decimal revenuePeriod;

        public ObservableCollection<ReservationDto> RecentReservations { get; } = new();

        public DashboardViewModel(ICustomerService customers, IRoomService rooms, IBookingService bookings, IReportService reports)
        {
            _customers = customers; _rooms = rooms; _bookings = bookings; _reports = reports;
        }

        [RelayCommand]
        private async Task Load()
        {
            // SỬA 1: Dùng Property "TotalCustomers" (viết hoa)
            TotalCustomers = _customers.Search(null).Count();

            var today = DateOnly.FromDateTime(DateTime.Today);
            var rooms = _rooms.Search(null).Select(r => r.RoomID).ToList();
            int busy = 0;
            foreach (var id in rooms)
                if (!await _bookings.RoomAvailableAsync(id, today, today, null)) busy++;

            // SỬA 2: Dùng Property "RoomsAvailable" (viết hoa)
            RoomsAvailable = rooms.Count - busy;

            // SỬA 3: Dùng Property "BookingsToday" (viết hoa)
            BookingsToday = _bookings.Search(null, today, today, null).Count();

            // Các dòng code này đã đúng (dùng property viết hoa)
            if (FromDate is not null && ToDate is not null)
            {
                var list = _reports.GetReservations(DateOnly.FromDateTime(FromDate.Value),
                                                    DateOnly.FromDateTime(ToDate.Value),
                                                    sortByBookingDateDesc: true);
                // Dòng này cũng đã đúng
                RevenuePeriod = list.Sum(x => x.TotalPrice ?? 0);
            }

            RecentReservations.Clear();
            foreach (var r in _bookings.Search(null, null, null, null)
                                      .OrderByDescending(x => x.BookingDate)
                                      .Take(10))
                RecentReservations.Add(r);
        }
    }
}