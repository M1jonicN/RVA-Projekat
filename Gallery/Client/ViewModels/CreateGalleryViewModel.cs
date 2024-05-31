using System;
using System.Windows;
using System.Windows.Input;
using Common.DbModels;
using Client.Helpers;
using System.ServiceModel;
using Common.Services;
using System.Linq;
using Client.Models;
using Client.Services;
using log4net;

namespace Client.ViewModels
{
    public class CreateGalleryViewModel : BaseViewModel
    {
        #region Fields

        private static readonly ILog log = LogManager.GetLogger(typeof(CreateGalleryViewModel));
        private Common.DbModels.Gallery _newGallery;
        private readonly IGalleryService _galleryService;
        public event EventHandler<Common.DbModels.Gallery> GalleryCreated;
        private string _loggedInUser;

        #endregion

        public CreateGalleryViewModel(string username)
        {
            NewGallery = new Common.DbModels.Gallery();
            CreateGalleryCommand = new RelayCommand(CreateGallery);
            _loggedInUser = username;

            var binding = new NetTcpBinding();
            var endpoint = new EndpointAddress("net.tcp://localhost:8086/Gallery");
            var channelFactory = new ChannelFactory<IGalleryService>(binding, endpoint);
            _galleryService = channelFactory.CreateChannel();

            log.Info("CreateGalleryViewModel initialized.");
            UserActionLoggerService.Instance.Log(_loggedInUser, "initialized CreateGalleryViewModel.");
        }

        public Common.DbModels.Gallery NewGallery
        {
            get => _newGallery;
            set
            {
                _newGallery = value;
                OnPropertyChanged();
            }
        }

        public ICommand CreateGalleryCommand { get; }

        #region Methods
        private bool AreFieldsValid()
        {
            // Check if all required fields are filled
            bool fieldsValid = !string.IsNullOrWhiteSpace(NewGallery.Address) &&
                               !string.IsNullOrWhiteSpace(NewGallery.MBR);

            if (!fieldsValid)
            {
                log.Warn("Attempted to create a gallery with invalid fields.");
                UserActionLoggerService.Instance.Log(_loggedInUser, "attempted to create a gallery with invalid fields.");
            }

            return fieldsValid;
        }

        private void CreateGallery()
        {
            if (!AreFieldsValid())
            {
                MessageBox.Show("All fields must be filled out.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                log.Warn("Attempt to create new Gallery failed due to invalid fields.");
                UserActionLoggerService.Instance.Log(_loggedInUser, "unsuccessfully created new Gallery due to invalid fields.");
                return;
            }

            try
            {
                bool result = _galleryService.CreateNewGallery(NewGallery);
                if (result)
                {
                    GalleryCreated?.Invoke(this, NewGallery);
                    MessageBox.Show("Gallery created successfully.");
                    log.Info("Successfully created new Gallery.");
                    UserActionLoggerService.Instance.Log(_loggedInUser, "successfully created new Gallery.");
                    Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
                }
                else
                {
                    MessageBox.Show("Failed to create gallery. A gallery with the same PIB might already exist.");
                    log.Warn("Failed to create new Gallery due to duplicate PIB.");
                    UserActionLoggerService.Instance.Log(_loggedInUser, "unsuccessfully created new Gallery.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while creating the new Gallery.");
                log.Error("An error occurred during the creation of a new Gallery.", ex);
                UserActionLoggerService.Instance.Log(_loggedInUser, $"unsuccessfully created new Gallery. Error: {ex.Message}");
            }
        }
        #endregion
    }
}
