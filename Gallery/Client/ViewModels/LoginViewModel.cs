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
using log4net;

namespace Client.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        #region Fields

        private static readonly ILog log = LogManager.GetLogger(typeof(LoginViewModel));
        private string _username;
        private string _errorMessage;
        private readonly ChannelFactory<IUserAuthenticationService> _channelFactory;
        private static UserActionsView _userActionsView;

        #endregion

        public LoginViewModel()
        {
            var binding = new NetTcpBinding();
            var endpoint = new EndpointAddress("net.tcp://localhost:8085/Authentifiaction");
            _channelFactory = new ChannelFactory<IUserAuthenticationService>(binding, endpoint);
            Username = "username";

            LoginCommand = new RelayCommand(Login);

            log.Info("LoginViewModel initialized.");
        }

        #region Properties
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
                log.Debug($"Username property changed to: {_username}");
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
                log.Debug($"Password property changed.");
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
                log.Debug($"ErrorMessage property changed to: {_errorMessage}");
            }
        }
        #endregion

        public ICommand LoginCommand { get; }

        #region Methods
        private void Login()
        {
            try
            {
                log.Info("Attempting to log in.");
                var UserAuthenticationServiceClient = _channelFactory.CreateChannel();
                User loggedInUser = UserAuthenticationServiceClient.Login(Username, Password);

                if (loggedInUser != null && loggedInUser.IsLoggedIn)
                {
                    log.Info($"User {Username} logged in successfully.");

                    // Open UserActionsView if not already open
                    if (_userActionsView == null)
                    {
                        _userActionsView = new UserActionsView
                        {
                            Height = 450,
                            Width = 800
                        };
                        var userActionsViewModel = new UserActionsViewModel();
                        _userActionsView.DataContext = userActionsViewModel;

                        _userActionsView.Closed += (s, e) => _userActionsView = null;
                        _userActionsView.Show();
                        log.Info("Successfully opened User Actions Window.");
                    }

                    UserActionLoggerService.Instance.Log(Username, " logged in successfully.");
                    log.Info(Username + " logged in successfully.");

                    var dashboardView = new DashboardView
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
                        log.Info("Dashboard window opened successfully.");
                    }
                }
                else
                {
                    ErrorMessage = "Invalid username or password";
                    log.Warn("Invalid username or password.");
                    UserActionLoggerService.Instance.Log(Username, " unsuccessfully logged in.");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
                log.Error("An error occurred during login.", ex);
                UserActionLoggerService.Instance.Log(Username, $" unsuccessfully logged in. Error: {ex.Message}");
            }
        }
        #endregion
    }
}
