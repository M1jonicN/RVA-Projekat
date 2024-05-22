using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Client.Helpers;
using Client.Views;
using Common.DbModels;
using Common.Interfaces;
using Common.Services;

namespace Client.ViewModels
{
    public class GalleryDetailsViewModel : BaseViewModel
    {
        private Gallery _gallery;
        private bool _isEditing;
        private readonly ChannelFactory<IAuthor> _channelFactoryAuthor;
        private readonly ChannelFactory<IWorkOfArt> _channelFactoryWorkOfArt;
        private readonly ChannelFactory<IGalleryService> _channelFactoryGallery;
        private readonly User _loggedInUser;
        private readonly DispatcherTimer _dispatcherTimer;

        public Gallery Gallery
        {
            get => _gallery;
            set
            {
                _gallery = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<WorkOfArt> WorkOfArts { get; set; }

        public string LoggedInUsername => _loggedInUser.Username;

        public GalleryDetailsViewModel(Gallery gallery, User loggedInUser)
        {
            _loggedInUser = loggedInUser;

            var bindingAuthor = new NetTcpBinding();
            var endpointAuthor = new EndpointAddress("net.tcp://localhost:8088/Author");
            _channelFactoryAuthor = new ChannelFactory<IAuthor>(bindingAuthor, endpointAuthor);

            var bindingWorkOfArt = new NetTcpBinding();
            var endpointWorkOfArt = new EndpointAddress("net.tcp://localhost:8087/WorkOfArt");
            _channelFactoryWorkOfArt = new ChannelFactory<IWorkOfArt>(bindingWorkOfArt, endpointWorkOfArt);

            var bindingGallery = new NetTcpBinding();
            var endpointGallery = new EndpointAddress("net.tcp://localhost:8086/Gallery");
            _channelFactoryGallery = new ChannelFactory<IGalleryService>(bindingGallery, endpointGallery);

            Gallery = gallery;
            WorkOfArts = new ObservableCollection<WorkOfArt>(gallery.WorkOfArts);

            FetchAuthorNames();

            EditCommand = new RelayCommand(Edit);
            SaveCommand = new RelayCommand(Save);
            DetailsWorkOfArtCommand = new RelayCommand<WorkOfArt>(DetailsWorkOfArt);
            DeleteWorkOfArtCommand = new RelayCommand<WorkOfArt>(DeleteWorkOfArt);

            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            _dispatcherTimer.Tick += (sender, args) => RefreshGallery();
            _dispatcherTimer.Start();
        }

        private void FetchAuthorNames()
        {
            var clientAuthor = _channelFactoryAuthor.CreateChannel();
            foreach (var workOfArt in WorkOfArts)
            {
                var authorName = clientAuthor.GetAuthorNameForWorkOfArt(workOfArt.ID, workOfArt.GalleryPIB);
                workOfArt.AuthorName = authorName;
            }
        }

        private void RefreshWorkOfArts()
        {
            var clientWorkOfArt = _channelFactoryWorkOfArt.CreateChannel();
            var updatedWorkOfArts = clientWorkOfArt.GetWorkOfArtsForGallery(Gallery.PIB);

            WorkOfArts.Clear();
            foreach (var workOfArt in updatedWorkOfArts)
            {
                WorkOfArts.Add(workOfArt);
            }
            FetchAuthorNames();
        }

        private void RefreshGallery()
        {
            if (IsEditing)
            {
                return; // Ako je u režimu uređivanja, ne osvežava galeriju
            }

            try
            {
                var clientGallery = _channelFactoryGallery.CreateChannel();
                var updatedGallery = clientGallery.GetGalleryByPIB(Gallery.PIB);
                if (updatedGallery != null)
                {
                    Gallery = updatedGallery;
                    Gallery.WorkOfArts = updatedGallery.WorkOfArts;
                    RefreshWorkOfArts();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to refresh gallery: {ex.Message}");
            }
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged();
            }
        }

        public ICommand DetailsWorkOfArtCommand { get; }
        public ICommand DeleteWorkOfArtCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }

        private void Edit()
        {
            IsEditing = true;
        }

        private void Save()
        {
            IsEditing = false;

            var clientGallery = _channelFactoryGallery.CreateChannel();
            clientGallery.SaveGalleryChanges(Gallery);
        }

        private void DetailsWorkOfArt(WorkOfArt workOfArt)
        {
            try
            {
                var clientAuthor = _channelFactoryAuthor.CreateChannel();
                var author = clientAuthor.GetAuthorByWorkOfArtId(workOfArt.AuthorID);
                var detailsViewModel = new WorkOfArtDetailsViewModel(workOfArt, author, _loggedInUser);
                var detailsWindow = new WorkOfArtDetailsWindow()
                {
                    DataContext = detailsViewModel
                };
                detailsWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void DeleteWorkOfArt(WorkOfArt workOfArt)
        {
            var clientWorkOfArt = _channelFactoryWorkOfArt.CreateChannel();
            try
            {
                clientWorkOfArt.DeleteWorkOfArt(workOfArt.ID);
                WorkOfArts.Remove(workOfArt);
                MessageBox.Show($"Deleted {workOfArt.ArtName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete {workOfArt.ArtName}: {ex.Message}");
            }
        }
    }
}
