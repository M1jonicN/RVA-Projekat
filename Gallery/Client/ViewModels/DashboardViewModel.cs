using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Client.Models;
using Client.Helpers;
using System.Collections.Generic;
using System.Windows;
using System;
using Common.Contracts;
using System.ServiceModel;

namespace Client.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly ChannelFactory<IAuthService> _channelFactory;
        private readonly Common.DbModels.User _loggedInUser;
        private ObservableCollection<Gallery> _galleries;
        private ObservableCollection<WorkOfArt> _workOfArts;
        private ObservableCollection<Author> _authors;
        private string _searchText;
        private string _loggedInUsername;

        public DashboardViewModel(Common.DbModels.User loggedInUser)
        {
            _loggedInUser = loggedInUser;
            LoggedInUsername = _loggedInUser.Username;

            var binding = new NetTcpBinding();
            var endpoint = new EndpointAddress("net.tcp://localhost:8085/Authentifiaction");
            _channelFactory = new ChannelFactory<IAuthService>(binding, endpoint);

            // Initialize collections with dummy data or fetch from service
            Galleries = new ObservableCollection<Gallery>();
            WorkOfArts = new ObservableCollection<WorkOfArt>();
            Authors = new ObservableCollection<Author>();

            SearchCommand = new RelayCommand(Search);
            LogoutCommand = new RelayCommand(Logout);

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

        private void LoadData()
        {
            // Dummy data, replace with actual data fetching
            Galleries.Add(new Gallery { Pib = "123456789", Address = "123 Gallery Street", Mbr = "987654321", WorkOfArts = new List<WorkOfArt>() });
            // Add more dummy data as needed
        }

        private void Search()
        {
            var filteredGalleries = Galleries.Where(g => g.Address.Contains(SearchText) || g.Pib.Contains(SearchText) || g.Mbr.Contains(SearchText)).ToList();
            Galleries = new ObservableCollection<Gallery>(filteredGalleries);
        }

        private void Logout()
        {
            try
            {
                var authServiceClient = _channelFactory.CreateChannel();
                bool isLoggedOut = authServiceClient.Logout(_loggedInUser.Username);

                if (isLoggedOut)
                {
                    MessageBox.Show($"Uspešna odjava {_loggedInUser.Username}!");
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
    }
}
