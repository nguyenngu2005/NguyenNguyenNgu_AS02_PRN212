using Business.Abstractions;
using Business.Dtos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NguyenNguyenNguWPF.Views; 
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NguyenNguyenNguWPF.ViewModels
{

    public partial class CustomersViewModel : ObservableObject
    {
        private readonly ICustomerService _customerSvc;

        [ObservableProperty]
        private string? keyword;

        public ObservableCollection<CustomerDto> Items { get; } = new();

        [ObservableProperty]
        private CustomerDto? selectedItem;

        public CustomersViewModel(ICustomerService customerSvc)
        {
            _customerSvc = customerSvc;
            Search();
        }

        [RelayCommand]
        private void Search()
        {
            Items.Clear();
            try
            {
                var data = _customerSvc.Search(Keyword).ToList();
                foreach (var item in data)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load customers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void New()
        {
            var dlg = App.AppHost!.Services.GetRequiredService<CustomerDialog>();
            dlg.Owner = Application.Current.MainWindow;

            dlg.DataContext = new CustomerDto();

            if (dlg.ShowDialog() == true)
            {
                var newDto = (CustomerDto)dlg.DataContext;

                // Gọi hàm Create và không cần chờ (fire-and-forget)
                _ = CreateAsync(newDto);
            }
        }

        private async Task CreateAsync(CustomerDto dto)
        {
            // Dùng (ok, error) tuple như trong CustomerService
            var (ok, error) = await _customerSvc.CreateAsync(dto);
            if (ok)
            {
                MessageBox.Show("Customer created successfully.");
                Search(); // Tải lại danh sách
            }
            else
            {
                MessageBox.Show($"Failed to create customer: {error}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Edit()
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("Please select a customer to edit.");
                return;
            }

            var dlg = App.AppHost!.Services.GetRequiredService<CustomerDialog>();
            dlg.Owner = Application.Current.MainWindow;

            // Gán DataContext là item được chọn
            // (CustomerDialog sẽ binding 2 chiều và tự động cập nhật)
            dlg.DataContext = SelectedItem;

            if (dlg.ShowDialog() == true)
            {
                var updatedDto = (CustomerDto)dlg.DataContext;
                _ = UpdateAsync(updatedDto);
            }
        }

        private async Task UpdateAsync(CustomerDto dto)
        {
            // Dùng (ok, error) tuple như trong CustomerService
            var (ok, error) = await _customerSvc.UpdateAsync(dto);
            if (ok)
            {
                MessageBox.Show("Customer updated successfully.");
                Search(); // Tải lại danh sách
            }
            else
            {
                MessageBox.Show($"Failed to update customer: {error}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task Delete()
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("Please select a customer to delete.");
                return;
            }

            if (MessageBox.Show($"Are you sure you want to delete '{SelectedItem.FullName}'?",
                                "Confirm Delete",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                // Dùng (ok, error) tuple như trong CustomerService
                var (ok, error) = await _customerSvc.DeleteAsync(SelectedItem.CustomerID);
                if (ok)
                {
                    MessageBox.Show("Customer deleted successfully.");
                    Search(); // Tải lại danh sách
                }
                else
                {
                    MessageBox.Show($"Failed to delete customer: {error}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}