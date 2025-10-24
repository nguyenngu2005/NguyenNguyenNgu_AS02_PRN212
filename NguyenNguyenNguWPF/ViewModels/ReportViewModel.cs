using Business.Abstractions;
using Business.Dtos; 
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32; 
using System;
using System.Collections.ObjectModel;
using System.IO; 
using System.Linq;
using System.Text; 
using System.Threading.Tasks;
using System.Windows;

namespace NguyenNguyenNguWPF.ViewModels 
{
    public partial class ReportViewModel : ObservableObject
    {
        private readonly IBookingService _svc;

        [ObservableProperty]
        private DateTime? fromDate = DateTime.Today.AddDays(-30);

        [ObservableProperty]
        private DateTime? toDate = DateTime.Today;

        public ObservableCollection<ReservationDto> Items { get; } = new();

        public ReportViewModel(IBookingService svc)
        {
            _svc = svc;
        }

        [RelayCommand]
        private async Task Load()
        {
            Items.Clear();

            DateOnly? start = FromDate.HasValue ? DateOnly.FromDateTime(FromDate.Value) : null;
            DateOnly? end = ToDate.HasValue ? DateOnly.FromDateTime(ToDate.Value) : null;

            try
            {
                var data = await Task.Run(() => _svc.Search(null, start, end, null).ToList());
                foreach (var item in data)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task Export()
        {
            if (!Items.Any())
            {
                MessageBox.Show("No data to export.", "Export Report", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var sfd = new SaveFileDialog
            {
                Filter = "CSV File (*.csv)|*.csv",
                Title = "Save Report As CSV",
                FileName = $"Report_{FromDate:yyyyMMdd}_to_{ToDate:yyyyMMdd}.csv"
            };

            if (sfd.ShowDialog() == true)
            {
                var csv = new StringBuilder();

                csv.AppendLine("BookingReservationID,BookingDate,CustomerName,TotalPrice");

                foreach (var item in Items)
                {
                    var customerName = $"\"{item.CustomerName?.Replace("\"", "\"\"")}\"";

                    csv.AppendLine($"{item.BookingReservationID},{item.BookingDate},{customerName},{item.TotalPrice}");
                }

                try
                {
                    await File.WriteAllTextAsync(sfd.FileName, csv.ToString());
                    MessageBox.Show($"Report saved successfully to:\n{sfd.FileName}", "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to save file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}