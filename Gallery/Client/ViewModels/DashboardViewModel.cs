﻿using System.Collections.ObjectModel;
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

namespace Client.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly ChannelFactory<IAuthService> _channelFactory;
        private readonly ChannelFactory<IGalleryService> _channelFactoryGallery;
        private readonly ChannelFactory<IWorkOfArt> _channelFactoryWOA;
        private readonly User _loggedInUser;
        private ObservableCollection<Gallery> _galleries;
        private ObservableCollection<Gallery> _allGalleries; // Dodato za čuvanje svih galerija
        private ObservableCollection<WorkOfArt> _workOfArts;
        private ObservableCollection<Author> _authors;
        private string _searchText;
        private string _loggedInUsername;
        private readonly DispatcherTimer _dispatcherTimer; // Dodato za tajmer
        private bool _isSearching; // Dodato za praćenje stanja pretrage

        private bool _isSearchByMBR;
        private bool _isSearchByPIB;
        private bool _isSearchByAddress;
        private bool _isSearchByParameters;

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
            CreateUserCommand = new RelayCommand(OpenCreateUserWindow);
            DetailsCommand = new RelayCommand<Gallery>(ShowDetails);
            DeleteCommand = new RelayCommand<Gallery>(DeleteGallery);
            CreateNewGalleryCommand = new RelayCommand(OpenCreateGalleryWindow);
            DuplicateGalleryCommand = new RelayCommand<Gallery>(DuplicateGallery);

            // Load data initially
            LoadData();

            // Initialize and start the DispatcherTimer
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Interval = TimeSpan.FromSeconds(1.5); // Set interval to 1.5 seconds
            _dispatcherTimer.Tick += (sender, args) => LoadData(); // Attach the LoadData method to the Tick event
            _dispatcherTimer.Start(); // Start the timer

            Application.Current.MainWindow.Closing += OnWindowClosing;
        }


        public ICommand DuplicateGalleryCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand EditUserCommand { get; }
        public ICommand CreateNewGalleryCommand { get; }
        public ICommand DetailsCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand CreateUserCommand { get; }

        private void DuplicateGallery(Gallery gallery)
        {
            // Kreiranje duboke kopije liste WorkOfArts
            var duplicatedWorkOfArts = new List<WorkOfArt>();
            if (gallery.WorkOfArts != null)
            {
                foreach (var workOfArt in gallery.WorkOfArts)
                {
                    duplicatedWorkOfArts.Add(new WorkOfArt
                    {
                        // Pretpostavljajući da WorkOfArt ima ove atribute, dodajte sve potrebne atribute za kopiranje
                        ID = workOfArt.ID,
                        ArtName = workOfArt.ArtName,
                        ArtMovement = workOfArt.ArtMovement,
                        Style = workOfArt.Style,
                        AuthorID = workOfArt.AuthorID,
                        AuthorName = workOfArt.AuthorName,
                        GalleryPIB = workOfArt.GalleryPIB,
                        IsDeleted = workOfArt.IsDeleted,
                        // Dodajte ostale atribute koje je potrebno kopirati
                    });
                }
            }

            // Kreiranje duplikata galerije
            var duplicateGallery = new Gallery
            {
                PIB = GenerateUniquePIB(),
                Address = gallery.Address,
                MBR = gallery.MBR,
                WorkOfArts = duplicatedWorkOfArts,
                IsDeleted = gallery.IsDeleted
            };

            // Dodavanje duplikata galerije u bazu podataka
            var clientGallery = _channelFactoryGallery.CreateChannel();
            clientGallery.CreateNewGallery(duplicateGallery);

            // Dodavanje duplikata galerije u obe kolekcije
            _allGalleries.Add(duplicateGallery);
            Galleries.Add(duplicateGallery);
        }


        private string GenerateUniquePIB()
        {
            string pib;
            do
            {
                pib = GeneratePIB();
            } while (_allGalleries.Any(g => g.PIB == pib));

            return pib;
        }

        private readonly Random _random = new Random();

        private string GeneratePIB()
        {
            // Generate a random number between 10000001 and 99999999
            int number = _random.Next(10000001, 99999999);

            // Convert the number to a string
            string pib = number.ToString("D8");

            // Calculate the control digit (simple example, customize as needed)
            int controlDigit = CalculateControlDigit(pib);

            // Append the control digit to the PIB
            return pib + controlDigit.ToString();
        }

        private int CalculateControlDigit(string pib)
        {
            // Implement a control digit calculation (example provided)
            // This is a simple checksum calculation, you may need a different algorithm
            int sum = 0;
            for (int i = 0; i < pib.Length; i++)
            {
                sum += (pib[i] - '0') * (i + 1);
            }

            // Modulus 11 to get a single digit
            int controlDigit = sum % 11;
            return controlDigit;
        }

        private void OpenCreateUserWindow()
        {
            if (_loggedInUser.UserType == Common.DbModels.UserType.Admin)
            {
                var createUserViewModel = new CreateUserViewModel();
                var createUserWindow = new CreateUserView
                {
                    DataContext = createUserViewModel,
                    Width = 400,
                    Height = 210
                };
                createUserWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Only Admin can add new User");
            }
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Logout();
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

        private void LoadData()
        {
            var clientGallery = _channelFactoryGallery.CreateChannel();
            var galleries = clientGallery.GetAllGalleries();

            _allGalleries.Clear();
            foreach (var gallery in galleries)
            {
                _allGalleries.Add(gallery);
            }

            // Always update Galleries collection
            if (!_isSearching)
            {
                Galleries.Clear();
                foreach (var gallery in _allGalleries)
                {
                    Galleries.Add(gallery);
                }
            }
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
            detailsWindow.Show();
        }

        private void DeleteGallery(Gallery gallery)
        {
            if (MessageBox.Show("Are you sure you want to delete this gallery?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                gallery.IsDeleted = true;
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
                _isSearching = false;
                Galleries = new ObservableCollection<Gallery>(_allGalleries);
            }
            else
            {
                _isSearching = true;

                var filteredGalleries = _allGalleries.AsQueryable();

                if (IsSearchByParameters)
                {
                    if (IsSearchByMBR)
                    {
                        filteredGalleries = filteredGalleries.Where(g => g.MBR.ToLower().Contains(SearchText.ToLower()));
                    }

                    if (IsSearchByPIB)
                    {
                        filteredGalleries = filteredGalleries.Where(g => g.PIB.ToLower().Contains(SearchText.ToLower()));
                    }

                    if (IsSearchByAddress)
                    {
                        filteredGalleries = filteredGalleries.Where(g => g.Address.ToLower().Contains(SearchText.ToLower()));
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
        }

        public void Logout()
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
                User user = authServiceClient.FindUser(_loggedInUser.Username);

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
            createGalleryWindow.Show();
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

        private void OnUserUpdated(object sender, User updatedUser)
        {
            _loggedInUser.Username = updatedUser.Username;
            LoggedInUsername = updatedUser.Username;
        }
    }
}
