using NguyenNguyenNguWPF.ViewModels;

namespace NguyenNguyenNguWPF.Views
{
    public partial class ReportView : System.Windows.Controls.UserControl
    {
        public ReportView(ReportViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}