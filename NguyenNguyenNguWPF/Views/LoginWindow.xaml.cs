using NguyenNguyenNguWPF.ViewModels;
using NguyenNguyenNguWPF.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input; 

namespace NguyenNguyenNguWPF.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _vm;

        public LoginWindow(LoginViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            DataContext = _vm;

            _vm.RequestClose += Vm_RequestClose;
        }

        private void Vm_RequestClose(object? sender, (string role, int? customerId) args)
        {

            var mainVM = App.AppHost!.Services.GetRequiredService<MainViewModel>();

            mainVM.Init(args.role, args.customerId);

            var mainWindow = new MainWindow(mainVM);

            Application.Current.MainWindow = mainWindow;

            mainWindow.Show();

            _vm.RequestClose -= Vm_RequestClose;
            this.Close();
        }
        private void PwdChanged(object sender, RoutedEventArgs e)
        {
            _vm.Password = ((PasswordBox)sender).Password;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}