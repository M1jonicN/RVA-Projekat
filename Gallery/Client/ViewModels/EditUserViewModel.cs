using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Client.Helpers;
using Client.Models;
using Common.DbModels;
using Common.Helper;
using Common.Contracts;
using System.Windows;
using Client.Services;

namespace Client.ViewModels
{
    public class EditUserViewModel : BaseViewModel
    {
        #region Fields
        public event EventHandler<Common.DbModels.User> UserUpdated;
        private readonly IAuthService _authService;
        private Common.DbModels.User _user;
        #endregion

        #region Properties
        public Common.DbModels.User User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged(nameof(Common.DbModels.User));
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

        public EditUserViewModel(Common.DbModels.User user, IAuthService authService)
        {
            _authService = authService;
            User = user;
            IsEditMode = false;
            TypeOfUser = user.UserType.ToString();
            SaveUserCommand = new RelayCommand(SaveUser);
            EditUserCommand = new RelayCommand(EditUser);
            LoadUserTypes();
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
                    UserActionLoggerService.Instance.Log(User.Username, " unsuccessfully edited user data, passwords doesn't match.");
                    return;
                }
                else if (!string.IsNullOrEmpty(NewPassword))
                {
                    User.PasswordHash = HashHelper.ConvertToHash(NewPassword);
                }
                else
                {
                    MessageBox.Show("U must enter password", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    UserActionLoggerService.Instance.Log(User.Username, " unsuccessfully edited user data, u must enter password.");
                    return;
                }

                var updated = _authService.SaveChanges(User);
                if (updated)
                {
                    MessageBox.Show("User saved successfully!");
                    UserActionLoggerService.Instance.Log(User.Username, " successfully edited user data.");

                    // Poziv događaja
                    UserUpdated?.Invoke(this, User);

                    Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
                }
                else
                {
                    MessageBox.Show("Failed to save user.");
                    UserActionLoggerService.Instance.Log(User.Username, " unsuccessfully edited user data, failed to save user.");
                }

                IsEditMode = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
                UserActionLoggerService.Instance.Log(User.Username, $" unsuccessfully edited user data, an error occurred: {ex.Message}.");


            }
        }

        private void EditUser()
        {
            IsEditMode = true;
        }
        #endregion
    }
}
