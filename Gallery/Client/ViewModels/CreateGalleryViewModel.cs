using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Common.DbModels;
using Common.Services;
using Client.Helpers;
using log4net;
using Client.Commands;
using Client.Services;
using System.ServiceModel;
using Common.Helpers;
using System.Net;


namespace Client.ViewModels
{
    public class CreateGalleryViewModel : BaseViewModel
    {
        #region Fields

        private static readonly ILog log = LogManager.GetLogger(typeof(CreateGalleryViewModel));
        private Gallery _newGallery;
        private readonly IGalleryService _galleryService;
        public event EventHandler<Gallery> GalleryCreated;
        private string _loggedInUser;
        private readonly Action<object, Gallery> _onGalleryCreated;

        #endregion

        #region Constructor

        public CreateGalleryViewModel(string username, EventHandler<Gallery> onGalleryCreated)
        {
            _newGallery = new Gallery();
            CreateGalleryCommand = new RelayCommand(OnCreateGalleryCommand);
            UndoCommand = new RelayCommand(Undo);
            RedoCommand = new RelayCommand(Redo);
            _loggedInUser = username;
            GalleryCreated += onGalleryCreated;

            var binding = new NetTcpBinding();
            var endpoint = new EndpointAddress("net.tcp://localhost:8086/Gallery");
            var channelFactory = new ChannelFactory<IGalleryService>(binding, endpoint);
            _galleryService = channelFactory.CreateChannel();

            log.Info("CreateGalleryViewModel initialized.");
            UserActionLoggerService.Instance.Log(_loggedInUser, "initialized CreateGalleryViewModel.");
        }

        #endregion

        #region Properties

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
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }

        #endregion

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

        private Gallery CreateGallery()
        {
            if (!AreFieldsValid())
            {
                MessageBox.Show("All fields must be filled out.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                log.Warn("Attempt to create new Gallery failed due to invalid fields.");
                UserActionLoggerService.Instance.Log(_loggedInUser, "unsuccessfully created new Gallery due to invalid fields.");
                return null;
            }

            try
            {
                var newGallery = new Gallery
                {
                    Address = NewGallery.Address,
                    MBR = NewGallery.MBR
                    // Set other necessary properties
                };
                return newGallery;
            }
            catch (Exception ex)
            {
                log.Error("Error occurred while creating new gallery.", ex);
                return null;
            }
        }

        private void OnCreateGalleryCommand()
        {
            var createdGallery = CreateGallery();
            if (createdGallery != null)
            {
                // Create AddGalleryCommand and execute it
                var addGalleryCommand = new AddGalleryCommand(createdGallery, _galleryService);
                Commands.CommandManager.ExecuteCommand(addGalleryCommand);
            }
                Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
        }
        private void Undo()
        {
            Commands.CommandManager.Undo();
        }

        private void Redo()
        {
            Commands.CommandManager.Redo();
        }

        #endregion
    }
}
