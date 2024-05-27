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

namespace Client.ViewModels
{
    public class CreateGalleryViewModel : BaseViewModel
    {
        #region Fields

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
            return !string.IsNullOrWhiteSpace(NewGallery.Address) &&
                   !string.IsNullOrWhiteSpace(NewGallery.MBR);
        }

        private void CreateGallery()
        {
            if (!AreFieldsValid())
            {
                MessageBox.Show("All fields must be filled out.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                UserActionLoggerService.Instance.Log(_loggedInUser, " unsuccessfully created new Gallery.");
                return;
            }

            bool result = _galleryService.CreateNewGallery(NewGallery);
            if (result)
            {
                GalleryCreated?.Invoke(this, NewGallery);
                MessageBox.Show("Gallery created successfully.");
                UserActionLoggerService.Instance.Log(_loggedInUser, " successfully created new Gallery.");
                Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
            }
            else
            {
                MessageBox.Show("Failed to create gallery. A gallery with the same PIB might already exist.");
                UserActionLoggerService.Instance.Log(_loggedInUser, " unsuccessfully created new Gallery.");
            }
        }
        #endregion
    }
}
