using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;
using Client.Helpers;
using Client.Views;
using Common.Contracts;

namespace Client.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _username;
        private string _errorMessage;
        private readonly ChannelFactory<IAuthService> _channelFactory;

        public LoginViewModel()
        {
            var binding = new NetTcpBinding();
            var endpoint = new EndpointAddress("net.tcp://localhost:8085/Authentifiaction");
            _channelFactory = new ChannelFactory<IAuthService>(binding, endpoint);

            LoginCommand = new RelayCommand(Login);
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }

        private async void Login()
        {
            try
            {
                var authServiceClient = _channelFactory.CreateChannel();
                bool result = authServiceClient.Login(Username, Password);
                if (result)
                {
                    MessageBox.Show("Uspesan Login!");
                    var dashboard = new DashboardView();
                    var dashboardViewModel = new DashboardViewModel();
                    dashboard.DataContext = dashboardViewModel;

                    var window = new Window
                    {
                        Content = dashboard,
                        Width = 800,
                        Height = 450,
                        Title = "Dashboard"
                    };
                    window.Show();
                }
                else
                {
                    ErrorMessage = "Invalid username or password";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
        }
    }
}
