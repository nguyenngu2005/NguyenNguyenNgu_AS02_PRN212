using System.Windows.Controls;
using NguyenNguyenNguWPF.ViewModels; 

namespace NguyenNguyenNguWPF.Views
{
    public partial class ProfileView : UserControl
    {
        public ProfileView(ProfileViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}