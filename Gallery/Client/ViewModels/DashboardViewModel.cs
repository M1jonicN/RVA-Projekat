using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Client.Helpers;
using System.Collections.Generic;
using System.Windows;
using System;
using System.ServiceModel;
using Client.Views;
using Common.DbModels;
using Common.Contracts;
using Common.Services;
using Common.Interfaces;
using System.Windows.Threading;
using Common.Helpers;
using Client.Services;
using log4net;
using Client.Commands;
using Client;
using Server.Services;
using System.ServiceModel.Channels;

namespace Client.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        #region Fields

        private static readonly ILog log = LogManager.GetLogger(typeof(DashboardViewModel));

        private readonly ChannelFactory<IUserAuthenticationService> _channelFactory;
        private readonly ChannelFactory<IGalleryService> _channelFactoryGallery;
        private readonly ChannelFactory<IWorkOfArtService> _channelFactoryWOA;
        private readonly ChannelFactory<IAuthorService> _channelFactoryAuthors;
        private IGalleryService galleryService;
        private readonly User _loggedInUser;
        private ObservableCollection<Gallery> _galleries;
        private ObservableCollection<Gallery> _allGalleries;
        private ObservableCollection<WorkOfArt> _workOfArts;
        private ObservableCollection<Author> _authors;
        private string _searchText;
        private string _loggedInUsername;
        private readonly DispatcherTimer _dispatcherTimer;
        private bool _isSearching;
        private bool _isSearchByMBR;
        private bool _isSearchByPIB;
        private bool _isSearchByAddress;
        private bool _isSearchByParameters;

        #endregion

        public DashboardViewModel(Common.DbModels.User loggedInUser)
        {
            _loggedInUser = loggedInUser;
            LoggedInUsername = _loggedInUser.Username;

            var binding = new NetTcpBinding();
            var endpoint = new EndpointAddress("net.tcp://localhost:8085/Authentifiaction");
            _channelFactory = new ChannelFactory<IUserAuthenticationService>(binding, endpoint);

            var bindingGallery = new NetTcpBinding();
            var endpointGallery = new EndpointAddress("net.tcp://localhost:8086/Gallery");
            _channelFactoryGallery = new ChannelFactory<IGalleryService>(bindingGallery, endpointGallery);

            var bindingWOA = new NetTcpBinding();
            var endpointWOA = new EndpointAddress("net.tcp://localhost:8087/WorkOfArt");
            _channelFactoryWOA = new ChannelFactory<IWorkOfArtService>(bindingWOA, endpointWOA);

            var bindingAuthors = new NetTcpBinding();
            var endpointAuthors = new EndpointAddress("net.tcp://localhost:8088/Author");
            _channelFactoryAuthors = new ChannelFactory<IAuthorService>(bindingAuthors, endpointAuthors);

            // Initialize collections with dummy data or fetch from service
            _allGalleries = new ObservableCollection<Gallery>();
            Galleries = new ObservableCollection<Gallery>();
            WorkOfArts = new ObservableCollection<WorkOfArt>();
            Authors = new ObservableCollection<Author>();


            SearchCommand = new RelayCommand(Search);
            LogoutCommand = new RelayCommand(Logout);
            EditUserCommand = new RelayCommand(Edit);
            CreateUserCommand = new RelayCommand(OpenCreateUserWindow);
            DetailsCommand = new RelayCommand<Gallery>(ShowDetails);
            DeleteCommand = new RelayCommand<Gallery>(DeleteGallery);
            CreateNewGalleryCommand = new RelayCommand(OpenCreateGalleryWindow);
            DuplicateGalleryCommand = new RelayCommand<Gallery>(DuplicateGallery);
            CreateNewAuthorCommand = new RelayCommand(OpenCreateAuthorWindow);
            CreateNewWorkOfArtCommand = new RelayCommand(OpenCreateWorkOfArtView);
            UndoCommand = new RelayCommand(Undo);
            RedoCommand = new RelayCommand(Redo);

            // Load data initially
            LoadData();

            // Initialize and start the DispatcherTimer
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Interval = TimeSpan.FromSeconds(1.5); // Set interval to 1.5 seconds
            _dispatcherTimer.Tick += (sender, args) => LoadData(); // Attach the LoadData method to the Tick event
            _dispatcherTimer.Start(); // Start the timer

            Application.Current.MainWindow.Closing += OnWindowClosing;
        }

        #region Commands
        public ICommand DuplicateGalleryCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand EditUserCommand { get; }
        public ICommand CreateNewGalleryCommand { get; }
        public ICommand DetailsCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand CreateUserCommand { get; }
        public ICommand CreateNewAuthorCommand { get; }
        public ICommand CreateNewWorkOfArtCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }

        #endregion

        #region Properties

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
                Search(); // Trigger search whenever search text changes
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

        public bool IsSearchByMBR
        {
            get => _isSearchByMBR;
            set
            {
                _isSearchByMBR = value;
                OnPropertyChanged();
                Search();
            }
        }

        public bool IsSearchByPIB
        {
            get => _isSearchByPIB;
            set
            {
                _isSearchByPIB = value;
                OnPropertyChanged();
                Search();
            }
        }

        public bool IsSearchByAddress
        {
            get => _isSearchByAddress;
            set
            {
                _isSearchByAddress = value;
                OnPropertyChanged();
                Search();
            }
        }

        public bool IsSearchByParameters
        {
            get => _isSearchByParameters;
            set
            {
                _isSearchByParameters = value;
                OnPropertyChanged();
                Search();
            }
        }

        public bool IsSearching { get => _isSearching; set => _isSearching = value; }

        #endregion

        #region Methods
        private void OpenCreateAuthorWindow()
        {
            var createAuthorView = new CreateAuthorView
            {
                DataContext = new CreateAuthorViewModel(_loggedInUser.Username)
            };
            log.Info($"{_loggedInUser.Username} successfully opened Create New Author Window.");
            createAuthorView.Show();
        }

        private void OpenCreateWorkOfArtView()
        {
            var createWorkOfArtView = new CreateWorkOfArtView
            {
                DataContext = new CreateWorkOfArtViewModel(_loggedInUser.Username) // Pass the logged-in user's username to the ViewModel
            };
            log.Info($"{_loggedInUser.Username} successfully opened Create New Work of Art Window.");
            createWorkOfArtView.Show();
        }

        private void DuplicateGallery(Gallery gallery)
        {
            galleryService = _channelFactoryGallery.CreateChannel();
            if (gallery != null)
            {
                // Create AddGalleryCommand and execute it
                var duplicateGalleryCommand = new DuplicateGalleryCommand(gallery, galleryService, Galleries);
                Commands.CommandManager.ExecuteCommand(duplicateGalleryCommand);
            }
        }

        private void OpenCreateUserWindow()
        {
            var createUserView = new CreateUserView
            {
                DataContext = new CreateUserViewModel(_loggedInUser.Username)
            };
            log.Info($"{_loggedInUser.Username} successfully opened Create New User Window.");
            createUserView.Show();
        }

        private void OpenCreateGalleryWindow()
        {
            var createGalleryView = new CreateGalleryView
            {
                DataContext = new CreateGalleryViewModel(_loggedInUser.Username, OnGalleryCreated)
            };

            var window = new Window
            {
                Content = createGalleryView,
                Title = "Create New Gallery",
                Width = 800,
                Height = 600
            };

            log.Info($"{_loggedInUser.Username} successfully opened Create New Gallery Window.");
            window.Show();
        }


        private void OnGalleryCreated(object sender, Gallery newGallery)
        {
            if (newGallery == null)
            {
                return;
            }

            // Add the new Gallery to ObservableCollection
            Galleries.Add(newGallery);
            _allGalleries.Add(newGallery);
            Commands.CommandManager._redoStack.Clear();           // Clear the redo stack

            log.Info($"{_loggedInUser.Username} successfully created a new gallery with PIB {newGallery.PIB}.");
        }


        private void DeleteGallery(Gallery gallery)
        {
            var clientGallery = _channelFactoryGallery.CreateChannel();
            gallery.IsDeleted = true;
            clientGallery.DeleteGallery(gallery.PIB);

            // Update local collections
           // _allGalleries.Remove(gallery);
           // Galleries.Remove(gallery);
            log.Info($"{_loggedInUser.Username} successfully deleted the gallery with PIB {gallery.PIB}.");
        }

        private void ShowDetails(Common.DbModels.Gallery gallery)
        {
            var clientWOA = _channelFactoryWOA.CreateChannel();
            var workOfArts = clientWOA.GetWorkOfArtsForGallery(gallery.PIB);
            gallery.WorkOfArts = new List<Common.DbModels.WorkOfArt>(workOfArts);

            var detailsViewModel = new GalleryDetailsViewModel(gallery, _loggedInUser);
            var detailsWindow = new GalleryDetailsWindow
            {
                DataContext = detailsViewModel,
                Width = 670,
                Height = 700
            };
            log.Info($"{_loggedInUser.Username} successfully opened Show Gallery Details Window for Gallery PIB: {gallery.PIB}.");
            detailsWindow.Show();
        }

        private void Search()
        {
            IsSearching = true;
            Galleries.Clear();

            var filteredGalleries = _allGalleries.Where(gallery =>
            {
                if (!string.IsNullOrEmpty(SearchText) && !gallery.Address.Contains(SearchText.ToLower()) &&
                    !gallery.MBR.Contains(SearchText.ToLower()) &&
                    !gallery.PIB.Contains(SearchText.ToLower()))
                {
                    return false;
                }

                if (IsSearchByMBR && !gallery.MBR.Equals(SearchText.ToLower()))
                {
                    return false;
                }

                if (IsSearchByPIB && !gallery.PIB.Equals(SearchText.ToLower()))
                {
                    return false;
                }

                if (IsSearchByAddress && !gallery.Address.Contains(SearchText.ToLower()))
                {
                    return false;
                }

                if (IsSearchByParameters && (string.IsNullOrEmpty(SearchText) ||
                                             (!gallery.MBR.Equals(SearchText, StringComparison.InvariantCultureIgnoreCase) &&
                                              !gallery.PIB.Equals(SearchText, StringComparison.InvariantCultureIgnoreCase) &&
                                              !gallery.Address.Contains(SearchText.ToLower()))))
                {
                    return false;
                }

                return true;
            }).ToList();

            foreach (var gallery in filteredGalleries)
            {
                Galleries.Add(gallery);
            }

            IsSearching = false;
        }

        public void Logout()
        {
            var client = _channelFactory.CreateChannel();
            client.Logout(_loggedInUser.Username);
            log.Info($"{_loggedInUser.Username} successfully logged out.");
            Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
        }

        private void Edit()
        {
            var client = _channelFactory.CreateChannel();
            var editUserView = new EditUserWindow
            {
                DataContext = new EditUserViewModel(_loggedInUser, client)
            };
            log.Info($"{_loggedInUser.Username} successfully opened Edit User Window.");
            editUserView.Show();
        }

        private void LoadData()
        {
            LoadGalleries();
            LoadWorksOfArt();
            LoadAuthors();
        }

        private void LoadGalleries()
        {
            try
            {
                var clientGallery = _channelFactoryGallery.CreateChannel();
                var galleries = clientGallery.GetAllGalleries();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _allGalleries.Clear();
                    Galleries.Clear();
                    foreach (var gallery in galleries)
                    {
                        _allGalleries.Add(gallery);
                        Galleries.Add(gallery);
                    }
                });
            }
            catch (Exception ex)
            {
                log.Error("Error occurred while loading galleries.", ex);
            }
        }

        private void LoadWorksOfArt()
        {
            try
            {
                var clientWOA = _channelFactoryWOA.CreateChannel();
                var worksOfArt = clientWOA.GetAllWorkOfArts();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    WorkOfArts.Clear();
                    foreach (var workOfArt in worksOfArt)
                    {
                        WorkOfArts.Add(workOfArt);
                    }
                });
            }
            catch (Exception ex)
            {
                log.Error("Error occurred while loading works of art.", ex);
            }
        }

        private void LoadAuthors()
        {
            try
            {
                var clientGallery = _channelFactoryAuthors.CreateChannel();
                var authors = clientGallery.GetAllAuthores();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Authors.Clear();
                    foreach (var author in authors)
                    {
                        Authors.Add(author);
                    }
                });
            }
            catch (Exception ex)
            {
                log.Error("Error occurred while loading authors.", ex);
            }
        }

        private void Undo()
        {
            if (Commands.CommandManager._undoStack.Count > 0)
            {
                Commands.CommandManager.Undo();

            }
        }

        private void Redo()
        {
            if (Commands.CommandManager._redoStack.Count > 0)
            {
                Commands.CommandManager.Redo();

            }
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Save state or handle necessary actions before the window closes
        }

        #endregion
    }
}
