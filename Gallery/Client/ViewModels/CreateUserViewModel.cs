using System;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;
using Client.Helpers;
using Client.Services;
using Common.Contracts;

namespace Client.ViewModels
{
    public class CreateUserViewModel : BaseViewModel
    {
        #region Fields

        private string _username;
        private string _password;
        private string _firstName;
        private string _lastName;
        private readonly ChannelFactory<IAuthService> _channelFactory;
        private string _loggedInUser;

        #endregion
        public CreateUserViewModel(string username)
        {
            _loggedInUser= username;
            var binding = new NetTcpBinding();
            var endpoint = new EndpointAddress("net.tcp://localhost:8085/Authentifiaction");
            _channelFactory = new ChannelFactory<IAuthService>(binding, endpoint);

            CreateUserCommand = new RelayCommand(CreateUser, CanCreateUser);
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
                var authServiceClient = _channelFactory.CreateChannel();
                bool isCreated = authServiceClient.Register(Username,Password,FirstName,LastName);

                if (isCreated)
                {
                    MessageBox.Show("User created successfully!");
                    UserActionLoggerService.Instance.Log(_loggedInUser, $" user {Username} created successfully.");
                    Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
                }
                else
                {
                    MessageBox.Show("Failed to create user.");
                    UserActionLoggerService.Instance.Log(_loggedInUser, $" failed to create user.");
                }
            }
            catch (Exception ex)
            {
                UserActionLoggerService.Instance.Log(_loggedInUser, " failed to create user.");
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private bool CanCreateUser()
        {
            return !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName);
        }
        #endregion
    }
}
