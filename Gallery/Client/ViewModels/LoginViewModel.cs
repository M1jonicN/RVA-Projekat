using System;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;
using Client.Helpers;
using Client.Models;
using Client.Views;
using Common.Contracts;
using Common.DbModels;

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
            Username = "username";

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

        private void Login()
        {
            try
            {
                var authServiceClient = _channelFactory.CreateChannel();
                Common.DbModels.User loggedInUser = authServiceClient.Login(Username, Password);
                if (loggedInUser != null && loggedInUser.IsLoggedIn)
                {
                    var dashboard = new DashboardView()
                    {
                        Height = 600,
                        Width = 900
                    };
                    var dashboardViewModel = new DashboardViewModel(loggedInUser);


                    // Subscribe to the Closed event of DashboardView
                   // dashboard.Closed += Dashboard_Closed;

                    dashboard.DataContext = dashboardViewModel;

                    Window currentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
                    if (currentWindow != null)
                    {
                        dashboard.Owner = currentWindow;
                        currentWindow.Hide();
                    }

                    dashboard.Show();
                    currentWindow?.Show(); 

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

       /* private void Dashboard_Closed(object sender, EventArgs e)
        {
            // Show the login window again when Dashboard is closed
            foreach (Window window in Application.Current.Windows)
            {
                if (window is LoginView)
                {
                    window.Show();
                    break;
                }
            }
        }*/
    }
}
