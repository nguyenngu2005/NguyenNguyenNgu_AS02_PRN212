using System.Windows;

namespace NguyenNguyenNguWPF.Views
{

    public partial class CustomerDialog : Window
    {
        public CustomerDialog()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

            this.DialogResult = true;
        }
    }
}