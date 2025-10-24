using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NguyenNguyenNguWPF.ViewModels;
using NguyenNguyenNguWPF.Views;
using System.Windows.Controls;

namespace NguyenNguyenNguWPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty] private string title = "FUMiniHotelSystem";
        [ObservableProperty] private bool isAdmin;
        [ObservableProperty] private bool isCustomer;
        [ObservableProperty] private UserControl? currentView;

        private readonly DashboardViewModel _dashboardVM;
        private readonly CustomersViewModel _customersVM;
        private readonly RoomsViewModel _roomsVM;
        private readonly ReservationsViewModel _reservationsVM;
        private readonly ReportViewModel _reportVM;
        private readonly ProfileViewModel _profileVM;
        // ĐÃ XÓA: HistoryViewModel

        public MainViewModel(
            DashboardViewModel dashboardVM,
            CustomersViewModel customersVM,
            RoomsViewModel roomsVM,
            ReservationsViewModel reservationsVM,
            ReportViewModel reportVM,
            ProfileViewModel profileVM)
        // ĐÃ XÓA: Tham số HistoryViewModel
        {
            _dashboardVM = dashboardVM; _customersVM = customersVM; _roomsVM = roomsVM;
            _reservationsVM = reservationsVM; _reportVM = reportVM; _profileVM = profileVM;
            // ĐÃ XÓA: Gán _historyVM
        }

        public void Init(string role, int? customerId)
        {
            IsAdmin = role.Equals("Admin", System.StringComparison.OrdinalIgnoreCase);
            IsCustomer = !IsAdmin;
            Title = IsAdmin ? "Admin" : "Customer";

            // SỬA LỖI: Bỏ comment ở dòng này để Profile View hoạt động
            _profileVM.SetCustomer(customerId);
            // ĐÃ XÓA: _historyVM.SetCustomer

            if (IsAdmin) ShowDashboard();
            else ShowProfile();
        }

        [RelayCommand]
        private void ShowDashboard()
            => CurrentView = App.AppHost!.Services.GetRequiredService<DashboardView>();
        [RelayCommand]
        private void ShowCustomers()
            => CurrentView = App.AppHost!.Services.GetRequiredService<CustomersView>();
        [RelayCommand]
        private void ShowRooms()
            => CurrentView = App.AppHost!.Services.GetRequiredService<RoomsView>();
        [RelayCommand]
        private void ShowReservations()
            => CurrentView = App.AppHost!.Services.GetRequiredService<ReservationsView>();
        [RelayCommand]
        private void ShowReport()
            => CurrentView = App.AppHost!.Services.GetRequiredService<ReportView>();
        [RelayCommand]
        private void ShowProfile()
            => CurrentView = App.AppHost!.Services.GetRequiredService<ProfileView>();

        // ĐÃ XÓA: Hàm ShowHistory()
    }
}