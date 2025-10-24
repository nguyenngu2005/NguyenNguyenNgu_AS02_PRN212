using System.Windows;

namespace NguyenNguyenNguWPF.Views
{
    public partial class RoomDialog : Window
    {
        public RoomDialog()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}