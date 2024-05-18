using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Client.Models;
using Client.Helpers;
using System.Collections.Generic;

namespace Client.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private ObservableCollection<Gallery> _galleries;
        private ObservableCollection<WorkOfArt> _workOfArts;
        private ObservableCollection<Author> _authors;
        private string _searchText;

        public DashboardViewModel()
        {
            // Initialize collections with dummy data or fetch from service
            Galleries = new ObservableCollection<Gallery>();
            WorkOfArts = new ObservableCollection<WorkOfArt>();
            Authors = new ObservableCollection<Author>();

            SearchCommand = new RelayCommand(Search);

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

        public ICommand SearchCommand { get; }

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
    }
}
