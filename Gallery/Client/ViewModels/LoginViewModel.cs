using System;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;
using Client.Helpers;
using Client.Views;
using Client.Services;
using Common.Contracts;
using Common.DbModels;

namespace Client.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _username;
        private string _errorMessage;
        private readonly ChannelFactory<IAuthService> _channelFactory;
        private static UserActionsView _userActionsView;

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
                    // Open UserActionsView if not already open
                    if (_userActionsView == null)
                    {
                        _userActionsView = new UserActionsView()
                        {
                            Height = 450,
                            Width = 800
                        };
                        var userActionsViewModel = new UserActionsViewModel();
                        _userActionsView.DataContext = userActionsViewModel;

                        _userActionsView.Closed += (s, e) => _userActionsView = null;
                        _userActionsView.Show();
                    }

                    UserActionLoggerService.Instance.Log(Username, " logged in successfully.");

                    var dashboardView = new DashboardView()
                    {
                        Height = 600,
                        Width = 900
                    };
                    var dashboardViewModel = new DashboardViewModel(loggedInUser);
                    dashboardView.DataContext = dashboardViewModel;

                    Window currentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
                    if (currentWindow != null)
                    {
                        dashboardView.Owner = currentWindow;
                        dashboardView.Show();
                    }
                }
                else
                {
                    ErrorMessage = "Invalid username or password";
                    UserActionLoggerService.Instance.Log(Username, " unsuccessfully logged in.");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
                UserActionLoggerService.Instance.Log(Username, $" unsuccessfully logged in. Error: {ex.Message}");
            }
        }
    }
}
