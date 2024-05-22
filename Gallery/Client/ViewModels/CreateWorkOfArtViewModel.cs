using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Windows.Input;
using System.Windows;
using Client.Helpers;
using Common.DbModels;
using Common.Interfaces;
using DbStyle = Common.DbModels.Style; // Alias za Common.DbModels.Style
using System.Linq;
using Common.Services;

namespace Client.ViewModels
{
    public class CreateWorkOfArtViewModel : BaseViewModel
    {
        private string _artName;
        private ArtMovement _selectedArtMovement;
        private DbStyle _selectedStyle;
        private string _selectedAuthorName;
        private string _selectedGalleryPIB;
        private readonly ChannelFactory<IAuthor> _channelFactoryAuthor;
        private readonly ChannelFactory<IWorkOfArt> _channelFactoryWoa;
        private readonly ChannelFactory<IGalleryService> _channelFactoryGallery;

        public CreateWorkOfArtViewModel()
        {
            var bindingAuthor = new NetTcpBinding();
            var endpointAuthor = new EndpointAddress("net.tcp://localhost:8088/Author");
            _channelFactoryAuthor = new ChannelFactory<IAuthor>(bindingAuthor, endpointAuthor);

            var bindingWoa = new NetTcpBinding();
            var endpointWoa = new EndpointAddress("net.tcp://localhost:8087/WorkOfArt");
            _channelFactoryWoa = new ChannelFactory<IWorkOfArt>(bindingWoa, endpointWoa);

            var bindingGallery = new NetTcpBinding();
            var endpointGallery = new EndpointAddress("net.tcp://localhost:8086/Gallery");
            _channelFactoryGallery = new ChannelFactory<IGalleryService>(bindingGallery, endpointGallery);

            var clientGallery = _channelFactoryGallery.CreateChannel();
            var galleries = clientGallery.GetAllGalleriesFromDb();

            // In galleriesWithPIB se nalaze sve galerije, njih isto ponuditi u obliku comboboxa na CreateWorkOfArt windowu.
            GalleryPIBs = new ObservableCollection<string>(galleries.Select(g => g.PIB));

            ArtMovements = new ObservableCollection<ArtMovement>(Enum.GetValues(typeof(ArtMovement)) as IEnumerable<ArtMovement>);
            Styles = new ObservableCollection<DbStyle>(Enum.GetValues(typeof(DbStyle)) as IEnumerable<DbStyle>);
            AuthorNames = new ObservableCollection<string>();

            LoadAuthors();

            SaveCommand = new RelayCommand(Save);
        }

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

        public ICommand SaveCommand { get; }

        private void LoadAuthors()
        {
            var clientAuthor = _channelFactoryAuthor.CreateChannel();
            var authors = clientAuthor.GetAllAuthores();

            foreach (var author in authors)
            {
                AuthorNames.Add($"{author.FirstName} {author.LastName}");
            }
        }

        private void Save()
        {
            // Prikazivanje poruke za testiranje
            if (!CanCreateWoa())
            {
                MessageBox.Show("Unsuccessfully create new Work of Art!");
                return;
            }

            var clientWoa = _channelFactoryWoa.CreateChannel();
            var clientAuthor = _channelFactoryAuthor.CreateChannel();
            var authors = clientAuthor.GetAllAuthores();
            int authorId = -1; // Inicijalizujemo authorId na -1 (ako ne pronađemo podudaranje)

            foreach (var author in authors)
            {
                string fullName = $"{author.FirstName} {author.LastName}";
                if (fullName.Equals(SelectedAuthorName, StringComparison.OrdinalIgnoreCase))
                {
                    authorId = author.ID;
                    break; // Prekidamo petlju ako smo pronašli podudaranje
                }
            }

            WorkOfArt newWoa = new WorkOfArt()
            {
                ArtName = ArtName,
                ArtMovement = SelectedArtMovement,
                Style = SelectedStyle,
                AuthorName = SelectedAuthorName,
                GalleryPIB = SelectedGalleryPIB, // Updated to use SelectedGalleryPIB
                AuthorID = authorId, // Postavljamo authorId
                IsDeleted = false
            };

            try
            {
                var success = clientWoa.CreateNewWorkOfArt(newWoa);
                if (success)
                {
                    MessageBox.Show("Successfully created new Work of Art!");
                    Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
                }
                else
                {
                    MessageBox.Show("Unsuccessfully create new Work of Art!");
                    Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private bool CanCreateWoa()
        {
            return !string.IsNullOrWhiteSpace(ArtName) &&
                   !string.IsNullOrWhiteSpace(SelectedAuthorName) &&
                   !string.IsNullOrWhiteSpace(SelectedArtMovement.ToString()) &&
                   !string.IsNullOrWhiteSpace(SelectedStyle.ToString()) &&
                   !string.IsNullOrWhiteSpace(SelectedGalleryPIB);
        }
    }
}
