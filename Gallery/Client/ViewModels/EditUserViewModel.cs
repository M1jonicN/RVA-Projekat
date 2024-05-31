using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Client.Helpers;
using Client.Models;
using Common.DbModels;
using Common.Helper;
using Common.Contracts;
using System.Windows;
using Client.Services;
using log4net;

namespace Client.ViewModels
{
    public class EditUserViewModel : BaseViewModel
    {
        #region Fields

        private static readonly ILog log = LogManager.GetLogger(typeof(EditUserViewModel));
        public event EventHandler<Common.DbModels.User> UserUpdated;
        private readonly IUserAuthenticationService _UserAuthenticationService;
        private Common.DbModels.User _user;

        #endregion

        #region Properties

        public Common.DbModels.User User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged(nameof(User));
            }
        }

        private string _newPassword;
        public string NewPassword
        {
            get { return _newPassword; }
            set
            {
                _newPassword = value;
                OnPropertyChanged(nameof(NewPassword));
                OnPropertyChanged(nameof(IsReadOnly));
            }
        }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set
            {
                _confirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
                OnPropertyChanged(nameof(IsReadOnly));
            }
        }

        private List<UserType> _userTypes;
        public List<UserType> UserTypes
        {
            get { return _userTypes; }
            set
            {
                _userTypes = value;
                OnPropertyChanged(nameof(UserTypes));
                OnPropertyChanged(nameof(IsReadOnly));
            }
        }

        private bool _isEditMode;
        public bool IsEditMode
        {
            get { return _isEditMode; }
            set
            {
                _isEditMode = value;
                OnPropertyChanged(nameof(IsEditMode));
                OnPropertyChanged(nameof(IsReadOnly));
            }
        }

        private string _typeOfUser;
        public string TypeOfUser
        {
            get { return _typeOfUser; }
            set
            {
                _typeOfUser = value;
                OnPropertyChanged(nameof(TypeOfUser));
                OnPropertyChanged(nameof(IsReadOnly));
            }
        }

        public bool IsReadOnly => !IsEditMode;

        #endregion

        #region Commands
        public ICommand EditUserCommand { get; }
        public ICommand SaveUserCommand { get; }

        #endregion

        public EditUserViewModel(Common.DbModels.User user, IUserAuthenticationService UserAuthenticationService)
        {
            _UserAuthenticationService = UserAuthenticationService;
            User = user;
            IsEditMode = false;
            TypeOfUser = user.UserType.ToString();
            SaveUserCommand = new RelayCommand(SaveUser);
            EditUserCommand = new RelayCommand(EditUser);
            LoadUserTypes();

            log.Info($"EditUserViewModel initialized for user {user.Username}.");
            UserActionLoggerService.Instance.Log(User.Username, " initialized EditUserViewModel.");
        }

        #region Methods

        private void LoadUserTypes()
        {
            UserTypes = Enum.GetValues(typeof(UserType)).Cast<UserType>().ToList();
        }

        private void SaveUser()
        {
            try
            {
                if (NewPassword != ConfirmPassword)
                {
                    MessageBox.Show("Passwords doesn't match", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    log.Warn($"User {User.Username} unsuccessfully edited user data, passwords doesn't match.");
                    UserActionLoggerService.Instance.Log(User.Username, " unsuccessfully edited user data, passwords doesn't match.");
                    return;
                }
                else if (!string.IsNullOrEmpty(NewPassword))
                {
                    User.PasswordHash = HashHelper.ConvertToHash(NewPassword);
                }
                else
                {
                    MessageBox.Show("You must enter a password", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    log.Warn($"User {User.Username} unsuccessfully edited user data, password is required.");
                    UserActionLoggerService.Instance.Log(User.Username, " unsuccessfully edited user data, password is required.");
                    return;
                }

                var updated = _UserAuthenticationService.SaveChanges(User);
                if (updated)
                {
                    MessageBox.Show("User saved successfully!");
                    log.Info($"User {User.Username} saved successfully.");
                    UserActionLoggerService.Instance.Log(User.Username, " successfully edited user data.");

                    // Raise the event
                    UserUpdated?.Invoke(this, User);

                    Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
                }
                else
                {
                    MessageBox.Show("Failed to save user.");
                    log.Warn($"Failed to save user {User.Username}.");
                    UserActionLoggerService.Instance.Log(User.Username, " unsuccessfully edited user data, failed to save user.");
                }

                IsEditMode = false;
            }
            catch (Exception ex)
            {
                log.Error($"An error occurred while saving user {User.Username}.", ex);
                UserActionLoggerService.Instance.Log(User.Username, $" unsuccessfully edited user data, an error occurred: {ex.Message}.");
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void EditUser()
        {
            IsEditMode = true;
        }

        #endregion
    }
}
