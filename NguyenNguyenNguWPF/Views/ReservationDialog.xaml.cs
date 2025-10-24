using Business.Dtos; 
using System.Windows;

namespace NguyenNguyenNguWPF.Views
{

    public partial class ReservationDialog : Window
    {
        private ReservationDto? _viewModel;

        public ReservationDialog()
        {
            InitializeComponent();
        }

        public void Init(ReservationDto reservation)
        {
            _viewModel = reservation;
            this.DataContext = _viewModel;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

            this.DialogResult = true; 
        }

        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.Details.Add(new BookingDetailDto
                {
                    StartDate = DateOnly.FromDateTime(DateTime.Today),
                    EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                    ActualPrice = 0
                });
            }
        }
    }
}