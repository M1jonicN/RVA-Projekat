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
using Common.Interfaces;
using System.Windows.Threading;

namespace Client.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly ChannelFactory<IAuthService> _channelFactory;
        private readonly ChannelFactory<IGalleryService> _channelFactoryGallery;
        private readonly ChannelFactory<IWorkOfArt> _channelFactoryWOA;
        private readonly Common.DbModels.User _loggedInUser;
        private ObservableCollection<Gallery> _galleries;
        private ObservableCollection<Gallery> _allGalleries; // Dodato za čuvanje svih galerija
        private ObservableCollection<WorkOfArt> _workOfArts;
        private ObservableCollection<Author> _authors;
        private string _searchText;
        private string _loggedInUsername;
        private readonly MyDbContext dbContext;
        private readonly DispatcherTimer _dispatcherTimer; // Dodato za tajmer

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

            var bindingWOA = new NetTcpBinding();
            var endpointWOA = new EndpointAddress("net.tcp://localhost:8087/WorkOfArt");
            _channelFactoryWOA = new ChannelFactory<IWorkOfArt>(bindingWOA, endpointWOA);

            // Initialize collections with dummy data or fetch from service
            _allGalleries = new ObservableCollection<Gallery>();
            Galleries = new ObservableCollection<Gallery>();
            WorkOfArts = new ObservableCollection<WorkOfArt>();
            Authors = new ObservableCollection<Author>();

            SearchCommand = new RelayCommand(Search);
            LogoutCommand = new RelayCommand(Logout);
            EditUserCommand = new RelayCommand(Edit);
            DetailsCommand = new RelayCommand<Gallery>(ShowDetails);
            DeleteCommand = new RelayCommand<Gallery>(DeleteGallery);
            CreateNewGalleryCommand = new RelayCommand(OpenCreateGalleryWindow);

            // Load data initially
            LoadData();

            // Initialize and start the DispatcherTimer
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Interval = TimeSpan.FromSeconds(2); // Set interval to 2 seconds
            _dispatcherTimer.Tick += (sender, args) => LoadData(); // Attach the LoadData method to the Tick event
            _dispatcherTimer.Start(); // Start the timer
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
        public ICommand DetailsCommand { get; }
        public ICommand DeleteCommand { get; }

        private void LoadData()
        {
            var galleries = new List<Gallery>();
            var clientGallery = _channelFactoryGallery.CreateChannel();

            galleries = clientGallery.GetAllGalleries();

            _allGalleries.Clear();
            Galleries.Clear();

            foreach (var gallery in galleries)
            {
                if (!gallery.IsDeleted)
                {
                    _allGalleries.Add(gallery);
                    Galleries.Add(gallery);
                }
            }
        }

        private void ShowDetails(Gallery gallery)
        {
            var clientWOA = _channelFactoryWOA.CreateChannel();
            var workOfArts = clientWOA.GetWorkOfArtsForGallery(gallery.PIB);
            gallery.WorkOfArts = new List<WorkOfArt>(workOfArts);

            var detailsViewModel = new GalleryDetailsViewModel(gallery);
            var detailsWindow = new GalleryDetailsWindow
            {
                DataContext = detailsViewModel
            };
            detailsWindow.Show();
        }

        private void DeleteGallery(Gallery gallery)
        {
            if (MessageBox.Show("Are you sure you want to delete this gallery?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _allGalleries.Remove(gallery);
                Galleries.Remove(gallery);

                var clientGallery = _channelFactoryGallery.CreateChannel();
                clientGallery.DeleteGallery(gallery.PIB);
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
