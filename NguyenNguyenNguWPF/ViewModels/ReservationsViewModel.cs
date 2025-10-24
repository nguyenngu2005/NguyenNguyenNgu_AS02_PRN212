using Business.Abstractions;
using Business.Dtos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NguyenNguyenNguWPF;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NguyenNguyenNguWPF.Views; // <-- SỬA LỖI: Thêm dòng này

// Lưu ý: Namespace của bạn có thể là "YourNameWPF.ViewModels"
// hoặc "NguyenNguyenNguWPF.ViewModels".
// Hãy đảm bảo nó khớp với dự án của bạn.
namespace NguyenNguyenNguWPF.ViewModels
{
    public partial class ReservationsViewModel : ObservableObject
    {
        private readonly IBookingService _svc;

        [ObservableProperty] private DateTime? fromDate = DateTime.Today.AddDays(-7);
        [ObservableProperty] private DateTime? toDate = DateTime.Today;
        [ObservableProperty] private string? keyword;

        public ObservableCollection<ReservationDto> Items { get; } = new();
        [ObservableProperty] private ReservationDto? selectedItem;

        public ReservationsViewModel(IBookingService svc) => _svc = svc;

        [RelayCommand]
        private async Task Search()
        {
            Items.Clear();

            // Dùng thuộc tính (viết hoa): FromDate, ToDate, Keyword
            // Bọc hàm đồng bộ trong Task.Run để chạy bất đồng bộ
            var data = await Task.Run(() => _svc.Search(null,
                FromDate.HasValue ? DateOnly.FromDateTime(FromDate.Value) : null,
                ToDate.HasValue ? DateOnly.FromDateTime(ToDate.Value) : null,
                Keyword).ToList());

            foreach (var i in data) Items.Add(i);
        }

        [RelayCommand]
        private void New()
        {
            // Giờ 'ReservationDialog' đã được nhận diện
            var dlg = App.AppHost!.Services.GetRequiredService<ReservationDialog>();
            dlg.Owner = Application.Current.MainWindow;
            dlg.Init(new ReservationDto { BookingDate = DateOnly.FromDateTime(DateTime.Today) });
            if (dlg.ShowDialog() == true) _ = Search();
        }

        [RelayCommand]
        private void Edit()
        {
            if (SelectedItem is null) return;
            var dlg = App.AppHost!.Services.GetRequiredService<ReservationDialog>();
            dlg.Owner = Application.Current.MainWindow;
            dlg.Init(SelectedItem);
            if (dlg.ShowDialog() == true) _ = Search();
        }

        [RelayCommand]
        private async Task Delete()
        {
            if (SelectedItem is null) return;
            if (MessageBox.Show($"Delete reservation {SelectedItem.BookingReservationID}?", "Confirm", MessageBoxButton.YesNo)
                == MessageBoxResult.Yes)
            {
                try
                {
                    await _svc.DeleteAsync(SelectedItem.BookingReservationID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message ?? "Delete failed");
                }

                await Search();
            }
        }
    }
}