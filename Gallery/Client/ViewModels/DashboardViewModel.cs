using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Client.Helpers;
using System.Collections.Generic;
using System.Windows;
using System;
using System.ServiceModel;
using Client.Views;
using Server;
using Common.DbModels;
using Common.Contracts;
using Common.Services;

namespace Client.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly ChannelFactory<IAuthService> _channelFactory;
        private readonly ChannelFactory<IGalleryService> _channelFactoryGallery;
        private readonly Common.DbModels.User _loggedInUser;
        private ObservableCollection<Gallery> _galleries;
        private ObservableCollection<Gallery> _allGalleries; // Dodato za čuvanje svih galerija
        private ObservableCollection<WorkOfArt> _workOfArts;
        private ObservableCollection<Author> _authors;
        private string _searchText;
        private string _loggedInUsername;
        private readonly MyDbContext dbContext;

        public DashboardViewModel(Common.DbModels.User loggedInUser)
        {
            _loggedInUser = loggedInUser;
            LoggedInUsername = _loggedInUser.Username;

            var binding = new NetTcpBinding();
            var endpoint = new EndpointAddress("net.tcp://localhost:8085/Authentifiaction");
            _channelFactory = new ChannelFactory<IAuthService>(binding, endpoint);

            var bindingGallery = new NetTcpBinding();
            var endpointGallery = new EndpointAddress("net.tcp://localhost:8086/Gallery");
            _channelFactoryGallery = new ChannelFactory<IGalleryService>(bindingGallery, endpointGallery);

            // Initialize collections with dummy data or fetch from service
            _allGalleries = new ObservableCollection<Gallery>();
            Galleries = new ObservableCollection<Gallery>();
            WorkOfArts = new ObservableCollection<WorkOfArt>();
            Authors = new ObservableCollection<Author>();

            SearchCommand = new RelayCommand(Search);
            LogoutCommand = new RelayCommand(Logout);
            EditUserCommand = new RelayCommand(Edit);
            CreateNewGalleryCommand = new RelayCommand(OpenCreateGalleryWindow);

            // Load data (this should be replaced with actual data fetching logic)
            LoadData();
        }

        public ObservableCollection<Gallery> Galleries
        {
            get => _galleries;
            set
            {
                _galleries = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<WorkOfArt> WorkOfArts
        {
            get => _workOfArts;
            set
            {
                _workOfArts = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Author> Authors
        {
            get => _authors;
            set
            {
                _authors = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }

        public string LoggedInUsername
        {
            get => _loggedInUsername;
            set
            {
                _loggedInUsername = value;
                OnPropertyChanged();
            }
        }

        public ICommand SearchCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand EditUserCommand { get; }
        public ICommand CreateNewGalleryCommand { get; }

        private void LoadData()
        {
            // Dummy data, replace with actual data fetching
            var galleries = new List<Gallery>();
            var clientGallery = _channelFactoryGallery.CreateChannel();

            galleries = clientGallery.GetAllGalleries();

            foreach (var gallery in galleries)
            {
                _allGalleries.Add(gallery);
                Galleries.Add(gallery);
            }
        }

        private void Search()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Galleries = new ObservableCollection<Gallery>(_allGalleries);
            }
            else
            {
                var filteredGalleries = _allGalleries
                    .Where(g => g.Address.ToLower().Contains(SearchText.ToLower())
                             || g.PIB.ToLower().Contains(SearchText.ToLower())
                             || g.MBR.ToLower().Contains(SearchText.ToLower()))
                    .ToList();
                Galleries = new ObservableCollection<Gallery>(filteredGalleries);
            }
        }

        private void Logout()
        {
            try
            {
                var authServiceClient = _channelFactory.CreateChannel();
                bool isLoggedOut = authServiceClient.Logout(_loggedInUser.Username);

                if (isLoggedOut)
                {
                    Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
                }
                else
                {
                    MessageBox.Show("Došlo je do greške pri odjavi.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void Edit()
        {
            try
            {
                var authServiceClient = _channelFactory.CreateChannel();
                Common.DbModels.User user = authServiceClient.FindUser(_loggedInUser.Username);

                if (user != null)
                {
                    var editUserViewModel = new EditUserViewModel(user, authServiceClient);
                    editUserViewModel.UserUpdated += OnUserUpdated;
                    var editUserWindow = new EditUserWindow
                    {
                        DataContext = editUserViewModel
                    };
                    editUserWindow.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void OpenCreateGalleryWindow()
        {
            var createGalleryViewModel = new CreateGalleryViewModel();
            createGalleryViewModel.GalleryCreated += OnGalleryCreated;

            var createGalleryWindow = new Window
            {
                Title = "Create New Gallery",
                Content = new CreateGalleryView
                {
                    DataContext = createGalleryViewModel
                },
                Width = 400,
                Height = 300
            };
            createGalleryWindow.ShowDialog();
        }

        private void OnGalleryCreated(object sender, Gallery newGallery)
        {
            _allGalleries.Add(newGallery);
            Galleries.Add(newGallery);

            // Find the window hosting CreateGalleryView and close it
            var createGalleryWindow = Application.Current.Windows
                .OfType<Window>()
                .SingleOrDefault(w => w.DataContext == sender);
            createGalleryWindow?.Close();
        }

        private void OnUserUpdated(object sender, Common.DbModels.User updatedUser)
        {
            _loggedInUser.Username = updatedUser.Username;
            LoggedInUsername = updatedUser.Username;
        }
    }
}
