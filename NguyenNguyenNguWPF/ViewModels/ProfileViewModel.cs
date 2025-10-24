using Business.Abstractions;
using Business.Dtos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NguyenNguyenNguWPF.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {
        private readonly ICustomerService _customerSvc;
        private readonly IBookingService _bookingSvc;
        private int? _customerId;

        [ObservableProperty]
        private CustomerDto? item;

        [ObservableProperty]
        private int bookingCount;

        public ObservableCollection<ReservationDto> BookingHistoryItems { get; } = new();

        public ProfileViewModel(ICustomerService customerSvc, IBookingService bookingSvc)
        {
            _customerSvc = customerSvc;
            _bookingSvc = bookingSvc;
        }

        public async void SetCustomer(int? customerId)
        {
            if (customerId == null)
            {
                Item = null;
                BookingCount = 0;
                BookingHistoryItems.Clear();
                return;
            }

            _customerId = customerId;
            await LoadCustomerData();
        }

        private async Task LoadCustomerData()
        {
            if (_customerId == null) return;

            BookingHistoryItems.Clear();

            try
            {
                Item = await _customerSvc.GetAsync(_customerId.Value);

                var bookings = await Task.Run(() => _bookingSvc.Search(_customerId, null, null, null).ToList());

                BookingCount = bookings.Count;
                foreach (var booking in bookings)
                {
                    BookingHistoryItems.Add(booking);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load customer data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Item = null;
                BookingCount = 0;
            }
        }

        [RelayCommand]
        private async Task Save()
        {
            if (Item == null) return;

            var (ok, error) = await _customerSvc.UpdateAsync(Item);

            if (ok)
            {
                MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Failed to update profile: {error}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}