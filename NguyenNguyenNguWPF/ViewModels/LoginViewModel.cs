using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Business.Abstractions;
using System;
using System.Threading.Tasks;

namespace NguyenNguyenNguWPF.ViewModels 
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _auth;

        [ObservableProperty] private string email = "";
        public string Password { get; set; } = "";
        public event EventHandler<(string role, int? customerId)>? RequestClose;
        public int? CustomerId { get; private set; }

        public LoginViewModel(IAuthService auth) => _auth = auth;

        [RelayCommand]
        private async Task Login()
        {
            (bool ok, string role, int? cid, string? error) =
                await _auth.LoginAsync(Email.Trim(), Password);

            if (!ok)
            {
                System.Windows.MessageBox.Show(error ?? "Login failed.");
                return;
            }

            CustomerId = cid;
            RequestClose?.Invoke(this, (role, cid));
        }
    }
}
