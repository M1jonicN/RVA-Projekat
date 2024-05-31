using System;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;
using Client.Helpers;
using Client.Services;
using Common.Contracts;
using log4net;

namespace Client.ViewModels
{
    public class CreateUserViewModel : BaseViewModel
    {
        #region Fields

        private static readonly ILog log = LogManager.GetLogger(typeof(CreateUserViewModel));
        private string _username;
        private string _password;
        private string _firstName;
        private string _lastName;
        private readonly ChannelFactory<IUserAuthenticationService> _channelFactory;
        private string _loggedInUser;

        #endregion
        public CreateUserViewModel(string username)
        {
            _loggedInUser = username;
            var binding = new NetTcpBinding();
            var endpoint = new EndpointAddress("net.tcp://localhost:8085/Authentifiaction");
            _channelFactory = new ChannelFactory<IUserAuthenticationService>(binding, endpoint);

            CreateUserCommand = new RelayCommand(CreateUser, CanCreateUser);

            log.Info("CreateUserViewModel initialized.");
            UserActionLoggerService.Instance.Log(_loggedInUser, " initialized CreateUserViewModel.");
        }
        #region Properties
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged();
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged();
            }
        }
        #endregion
        public ICommand CreateUserCommand { get; }

        #region Methods
        private void CreateUser()
        {
            try
            {
                log.Info("Attempting to create user.");
                UserActionLoggerService.Instance.Log(_loggedInUser, $" attempting to create user {Username}.");
                var UserAuthenticationServiceClient = _channelFactory.CreateChannel();
                bool isCreated = UserAuthenticationServiceClient.Register(Username, Password, FirstName, LastName);

                if (isCreated)
                {
                    MessageBox.Show("User created successfully!");
                    log.Info($"User {Username} created successfully.");
                    UserActionLoggerService.Instance.Log(_loggedInUser, $" user {Username} created successfully.");
                    Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
                }
                else
                {
                    MessageBox.Show("Failed to create user.");
                    log.Warn($"Failed to create user {Username}.");
                    UserActionLoggerService.Instance.Log(_loggedInUser, $" failed to create user {Username}.");
                }
            }
            catch (Exception ex)
            {
                log.Error($"An error occurred while creating user {Username}.", ex);
                UserActionLoggerService.Instance.Log(_loggedInUser, $" failed to create user {Username}. Error: {ex.Message}");
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private bool CanCreateUser()
        {
            bool canCreate = !string.IsNullOrWhiteSpace(Username) &&
                             !string.IsNullOrWhiteSpace(Password) &&
                             !string.IsNullOrWhiteSpace(FirstName) &&
                             !string.IsNullOrWhiteSpace(LastName);

            if (!canCreate)
            {
                log.Warn($"Attempted to create user {Username} with invalid fields.");
                UserActionLoggerService.Instance.Log(_loggedInUser, " attempted to create a user with invalid fields.");
            }

            return canCreate;
        }
        #endregion
    }
}
