using System;
using System.Windows;
using System.Windows.Input;
using Common.DbModels;
using Client.Helpers;
using System.ServiceModel;
using Common.Services;
using System.Linq;

namespace Client.ViewModels
{
    public class CreateGalleryViewModel : BaseViewModel
    {
        private Gallery _newGallery;
        private readonly IGalleryService _galleryService;

        public event EventHandler<Gallery> GalleryCreated;

        public CreateGalleryViewModel()
        {
            NewGallery = new Gallery();
            CreateGalleryCommand = new RelayCommand(CreateGallery);

            var binding = new NetTcpBinding();
            var endpoint = new EndpointAddress("net.tcp://localhost:8086/Gallery");
            var channelFactory = new ChannelFactory<IGalleryService>(binding, endpoint);
            _galleryService = channelFactory.CreateChannel();
        }

        public Gallery NewGallery
        {
            get => _newGallery;
            set
            {
                _newGallery = value;
                OnPropertyChanged();
            }
        }

        public ICommand CreateGalleryCommand { get; }

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
                return;
            }

            bool result = _galleryService.CreateNewGallery(NewGallery);
            if (result)
            {
                GalleryCreated?.Invoke(this, NewGallery);
                MessageBox.Show("Gallery created successfully.");
                Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
            }
            else
            {
                MessageBox.Show("Failed to create gallery. A gallery with the same PIB might already exist.");
            }
        }
    }
}
