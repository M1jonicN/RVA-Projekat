using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Windows.Input;
using System.Windows;
using Client.Helpers;
using Common.DbModels;
using Common.Interfaces;
using DbStyle = Common.DbModels.Style;
using System.Linq;
using Common.Services;
using Client.Services;
using log4net;

namespace Client.ViewModels
{
    public class CreateWorkOfArtViewModel : BaseViewModel
    {

        #region Fields
        private static readonly ILog log = LogManager.GetLogger(typeof(CreateWorkOfArtViewModel));
        private string _artName;
        private ArtMovement _selectedArtMovement;
        private DbStyle _selectedStyle;
        private string _selectedAuthorName;
        private string _selectedGalleryPIB;
        private readonly ChannelFactory<IAuthorService> _channelFactoryAuthor;
        private readonly ChannelFactory<IWorkOfArtService> _channelFactoryWoa;
        private readonly ChannelFactory<IGalleryService> _channelFactoryGallery;
        private readonly string _loggedInUser;
        #endregion

        public CreateWorkOfArtViewModel()
        {
        }

        public CreateWorkOfArtViewModel(string loggedInUser)
        {
            _loggedInUser = loggedInUser;
            log.Info($"{_loggedInUser} initialized CreateWorkOfArtViewModel.");

            var bindingAuthor = new NetTcpBinding();
            var endpointAuthor = new EndpointAddress("net.tcp://localhost:8088/Author");
            _channelFactoryAuthor = new ChannelFactory<IAuthorService>(bindingAuthor, endpointAuthor);

            var bindingWoa = new NetTcpBinding();
            var endpointWoa = new EndpointAddress("net.tcp://localhost:8087/WorkOfArt");
            _channelFactoryWoa = new ChannelFactory<IWorkOfArtService>(bindingWoa, endpointWoa);

            var bindingGallery = new NetTcpBinding();
            var endpointGallery = new EndpointAddress("net.tcp://localhost:8086/Gallery");
            _channelFactoryGallery = new ChannelFactory<IGalleryService>(bindingGallery, endpointGallery);

            var clientGallery = _channelFactoryGallery.CreateChannel();
            var galleries = clientGallery.GetAllGalleriesFromDb();

            GalleryPIBs = new ObservableCollection<string>(galleries.Select(g => g.PIB));
            ArtMovements = new ObservableCollection<ArtMovement>(Enum.GetValues(typeof(ArtMovement)).Cast<ArtMovement>());
            Styles = new ObservableCollection<DbStyle>(Enum.GetValues(typeof(DbStyle)).Cast<DbStyle>());
            AuthorNames = new ObservableCollection<string>();

            LoadAuthors();
            SaveCommand = new RelayCommand(Save);
        }

        #region Properties
        public string ArtName
        {
            get => _artName;
            set => SetProperty(ref _artName, value);
        }

        public ArtMovement SelectedArtMovement
        {
            get => _selectedArtMovement;
            set => SetProperty(ref _selectedArtMovement, value);
        }

        public DbStyle SelectedStyle
        {
            get => _selectedStyle;
            set => SetProperty(ref _selectedStyle, value);
        }

        public string SelectedAuthorName
        {
            get => _selectedAuthorName;
            set => SetProperty(ref _selectedAuthorName, value);
        }

        public string SelectedGalleryPIB
        {
            get => _selectedGalleryPIB;
            set => SetProperty(ref _selectedGalleryPIB, value);
        }

        public ObservableCollection<ArtMovement> ArtMovements { get; }
        public ObservableCollection<DbStyle> Styles { get; }
        public ObservableCollection<string> AuthorNames { get; }
        public ObservableCollection<string> GalleryPIBs { get; }
        #endregion

        public ICommand SaveCommand { get; }

        #region Methods
        private void LoadAuthors()
        {
            var clientAuthor = _channelFactoryAuthor.CreateChannel();
            var authors = clientAuthor.GetAllAuthores();

            foreach (var author in authors)
            {
                AuthorNames.Add($"{author.FirstName} {author.LastName}");
            }

            log.Info($"{_loggedInUser} loaded authors.");
        }

        private void Save()
        {
            if (!CanCreateWoa())
            {
                MessageBox.Show("Unsuccessfully create new Work of Art!");
                log.Warn($"{_loggedInUser} unsuccessfully attempted to create new Work of Art due to invalid input.");
                return;
            }

            var clientWoa = _channelFactoryWoa.CreateChannel();
            var clientAuthor = _channelFactoryAuthor.CreateChannel();
            var authors = clientAuthor.GetAllAuthores();
            int authorId = -1;

            foreach (var author in authors)
            {
                string fullName = $"{author.FirstName} {author.LastName}";
                if (fullName.Equals(SelectedAuthorName, StringComparison.OrdinalIgnoreCase))
                {
                    authorId = author.ID;
                    break;
                }
            }

            var newWoa = new WorkOfArt
            {
                ArtName = ArtName,
                ArtMovement = SelectedArtMovement,
                Style = SelectedStyle,
                AuthorName = SelectedAuthorName,
                GalleryPIB = SelectedGalleryPIB,
                AuthorID = authorId,
                IsDeleted = false
            };

            try
            {
                var success = clientWoa.CreateNewWorkOfArt(newWoa);
                if (success)
                {
                    MessageBox.Show("Successfully created new Work of Art!");
                    log.Info($"{_loggedInUser} successfully created new Work of Art with name: {ArtName}.");
                    Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
                }
                else
                {
                    MessageBox.Show("Unsuccessfully create new Work of Art!");
                    log.Warn($"{_loggedInUser} unsuccessfully created new Work of Art.");
                    Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error($"{_loggedInUser} encountered an error while creating new Work of Art. Error: {ex.Message}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanCreateWoa()
        {
            return !string.IsNullOrWhiteSpace(ArtName) &&
                   !string.IsNullOrWhiteSpace(SelectedAuthorName) &&
                   SelectedArtMovement != default &&
                   SelectedStyle != default &&
                   !string.IsNullOrWhiteSpace(SelectedGalleryPIB);
        }
        #endregion
    }
}
