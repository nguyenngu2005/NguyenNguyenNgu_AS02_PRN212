using Business.Abstractions;
using Business.Dtos; // Giả định bạn có RoomDto tại đây
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NguyenNguyenNguWPF.Views; // Cần using này để gọi RoomDialog
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NguyenNguyenNguWPF.ViewModels
{
    // SỬA LỖI: Luôn thêm "public"
    public partial class RoomsViewModel : ObservableObject
    {
        private readonly IRoomService _roomSvc;

        // Binding với TextBox tìm kiếm
        [ObservableProperty]
        private string? keyword;

        // Binding với DataGrid
        public ObservableCollection<RoomDto> Items { get; } = new();

        // Binding với SelectedItem của DataGrid
        [ObservableProperty]
        private RoomDto? selectedItem;

        public RoomsViewModel(IRoomService roomSvc)
        {
            _roomSvc = roomSvc;
            Search(); // Tải dữ liệu ban đầu khi khởi tạo
        }

        [RelayCommand]
        private void Search()
        {
            Items.Clear();
            try
            {
                // Giả định hàm Search() của RoomService là đồng bộ (giống CustomerService)
                var data = _roomSvc.Search(Keyword).ToList();
                foreach (var item in data)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load rooms: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void New()
        {
            var dlg = App.AppHost!.Services.GetRequiredService<RoomDialog>();
            dlg.Owner = Application.Current.MainWindow;

            // (Giả định RoomDialog có hàm Init() để gán DataContext)
            // dlg.Init(new RoomDto()); // Tạo một DTO mới

            // Nếu RoomDialog dùng DataContext trống:
            dlg.DataContext = new RoomDto();

            if (dlg.ShowDialog() == true)
            {
                // Lấy dữ liệu từ DataContext của Dialog
                var newDto = (RoomDto)dlg.DataContext;

                // Gọi service để tạo mới
                // Dùng _ = ... để gọi hàm async mà không cần await (fire-and-forget)
                _ = CreateAsync(newDto);
            }
        }

        private async Task CreateAsync(RoomDto dto)
        {
            // Giả định hàm CreateAsync trả về (bool, string?)
            var (ok, error) = await _roomSvc.CreateAsync(dto);
            if (ok)
            {
                MessageBox.Show("Room created successfully.");
                Search(); // Tải lại danh sách
            }
            else
            {
                MessageBox.Show($"Failed to create room: {error}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Edit()
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("Please select a room to edit.");
                return;
            }

            var dlg = App.AppHost!.Services.GetRequiredService<RoomDialog>();
            dlg.Owner = Application.Current.MainWindow;

            // Gán DataContext là item được chọn
            dlg.DataContext = SelectedItem;

            if (dlg.ShowDialog() == true)
            {
                // Lấy dữ liệu đã bị chỉnh sửa từ DataContext
                var updatedDto = (RoomDto)dlg.DataContext;

                _ = UpdateAsync(updatedDto);
            }
            // (Nếu người dùng nhấn Cancel, không làm gì cả)
        }

        private async Task UpdateAsync(RoomDto dto)
        {
            // Giả định hàm UpdateAsync trả về (bool, string?)
            var (ok, error) = await _roomSvc.UpdateAsync(dto);
            if (ok)
            {
                MessageBox.Show("Room updated successfully.");
                Search(); // Tải lại danh sách
            }
            else
            {
                MessageBox.Show($"Failed to update room: {error}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task Delete()
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("Please select a room to delete.");
                return;
            }

            // Xác nhận trước khi xóa
            if (MessageBox.Show($"Are you sure you want to delete Room '{SelectedItem.RoomNumber}'?",
                                "Confirm Delete",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                // Giả định hàm DeleteAsync trả về (bool, string?)
                var (ok, error) = await _roomSvc.DeleteAsync(SelectedItem.RoomID);
                if (ok)
                {
                    MessageBox.Show("Room deleted successfully.");
                    Search(); // Tải lại danh sách
                }
                else
                {
                    MessageBox.Show($"Failed to delete room: {error}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}