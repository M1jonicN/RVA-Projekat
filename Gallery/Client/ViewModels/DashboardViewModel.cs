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

        public DashboardViewModel(User loggedInUser)
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
                DataContext = new CreateAuthorViewModel(_loggedInUser.Username),
                Width = 400,
                Height = 390
            };
            log.Info($"{_loggedInUser.Username} successfully opened Create New Author Window.");
            createAuthorView.Show();
        }

        private void OpenCreateWorkOfArtView()
        {
            var createWorkOfArtView = new CreateWorkOfArtView
            {
                DataContext = new CreateWorkOfArtViewModel(_loggedInUser.Username),
                Height = 280,
                Width = 400
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
                UserActionLoggerService.Instance.Log(_loggedInUser.Username, $"duplicated gallery with PIB: {gallery.PIB}.");
            }
        }

        private void OpenCreateUserWindow()
        {
            if (_loggedInUser.UserType == 0)
            {
                var createUserView = new CreateUserView
                {
                    DataContext = new CreateUserViewModel(_loggedInUser.Username),
                    Height = 250,
                    Width = 400
                };
                log.Info($"{_loggedInUser.Username} successfully opened Create New User Window.");
                createUserView.Show();
            }
            else
            {
                MessageBox.Show("Only admin can create new user!");
            }
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
                Width = 450,
                Height = 250
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

            Galleries.Add(newGallery);
            _allGalleries.Add(newGallery);
            Commands.CommandManager._redoStack.Clear();

            log.Info($"{_loggedInUser.Username} successfully created a new gallery with PIB {newGallery.PIB}.");
            UserActionLoggerService.Instance.Log(_loggedInUser.Username, $"created a new gallery with PIB: {newGallery.PIB}.");
        }


        private void DeleteGallery(Gallery gallery)
        {
            var clientGallery = _channelFactoryGallery.CreateChannel();

            var deleteGalleryCommand = new DeleteGalleryCommand(gallery, clientGallery);
            Commands.CommandManager.ExecuteCommand(deleteGalleryCommand);

            log.Info($"{_loggedInUser.Username} successfully deleted the gallery with PIB {gallery.PIB}.");
            UserActionLoggerService.Instance.Log(_loggedInUser.Username, $"deleted the gallery with PIB: {gallery.PIB}.");
        }

        private void ShowDetails(Gallery gallery)
        {
            var clientWOA = _channelFactoryWOA.CreateChannel();
            var workOfArts = clientWOA.GetWorkOfArtsForGallery(gallery.PIB);
            gallery.WorkOfArts = new List<WorkOfArt>(workOfArts);

            var detailsViewModel = new GalleryDetailsViewModel(gallery, _loggedInUser);
            var detailsWindow = new GalleryDetailsWindow
            {
                DataContext = detailsViewModel,
                Width = 670,
                Height = 700
            };
            log.Info($"{_loggedInUser.Username} successfully opened Show Gallery Details Window for Gallery PIB: {gallery.PIB}.");
            detailsWindow.Show();
            UserActionLoggerService.Instance.Log(_loggedInUser.Username, $"viewed details for gallery with PIB: {gallery.PIB}.");
        }



        private void Search()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                IsSearching = false;
                Galleries = new ObservableCollection<Gallery>(_allGalleries);
            }
            else
            {
                IsSearching = true;

                var filteredGalleries = _allGalleries.AsQueryable();

                if (IsSearchByParameters)
                {
                    if (IsSearchByMBR)
                    {
                        filteredGalleries = filteredGalleries.Where(g => g.MBR.ToLower().Contains(SearchText.ToLower()));
                        log.Info($"{_loggedInUser.Username} searched data by MBR.");
                    }

                    if (IsSearchByPIB)
                    {
                        filteredGalleries = filteredGalleries.Where(g => g.PIB.ToLower().Contains(SearchText.ToLower()));
                        log.Info($"{_loggedInUser.Username} searched data by PIB.");
                    }

                    if (IsSearchByAddress)
                    {
                        filteredGalleries = filteredGalleries.Where(g => g.Address.ToLower().Contains(SearchText.ToLower()));
                        log.Info($"{_loggedInUser.Username} searched data by Address.");
                    }
                }
                else
                {
                    filteredGalleries = filteredGalleries.Where(g => g.Address.ToLower().Contains(SearchText.ToLower())
                             || g.PIB.ToLower().Contains(SearchText.ToLower())
                             || g.MBR.ToLower().Contains(SearchText.ToLower()));
                }

                Galleries = new ObservableCollection<Gallery>(filteredGalleries.ToList());
            }

            UserActionLoggerService.Instance.Log(_loggedInUser.Username, $"performed a search with text: {SearchText}.");
        }

        public void Logout()
        {
            var client = _channelFactory.CreateChannel();
            client.Logout(_loggedInUser.Username);
            log.Info($"{_loggedInUser.Username} successfully logged out.");
            UserActionLoggerService.Instance.Log(_loggedInUser.Username, "logged out.");
            Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
        }

        private void Edit()
        {
            var client = _channelFactory.CreateChannel();
            var editUserView = new EditUserWindow
            {
                DataContext = new EditUserViewModel(_loggedInUser, client),
                Width = 400,
                Height = 400
            };
            log.Info($"{_loggedInUser.Username} successfully opened Edit User Window.");
            editUserView.Show();
            UserActionLoggerService.Instance.Log(_loggedInUser.Username, "opened Edit User Window.");
        }

        private void LoadData()
        {
            LoggedInUsername = _loggedInUser.Username;
            LoadGalleries();
            LoadWorksOfArt();
            LoadAuthors();
        }


        private void LoadGalleries()
        {
            if (!IsSearching)
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
                catch (FaultException ex)
                {
                    log.Error("FaultException occurred while loading galleries.", ex);
                    // Handle FaultException specific logic
                }
                catch (Exception ex)
                {
                    log.Error("Error occurred while loading galleries.", ex);
                    // Handle general exception logic
                }
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
                UserActionLoggerService.Instance.Log(_loggedInUser.Username, "performed an undo action.");
            }
        }

        private void Redo()
        {
            if (Commands.CommandManager._redoStack.Count > 0)
            {
                Commands.CommandManager.Redo();
                UserActionLoggerService.Instance.Log(_loggedInUser.Username, "performed a redo action.");
            }
        }


        #endregion
    }
}
