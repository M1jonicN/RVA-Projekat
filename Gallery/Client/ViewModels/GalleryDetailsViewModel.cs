using Common.DbModels;
using Common.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;
using Client.Helpers;

namespace Client.ViewModels
{
    public class GalleryDetailsViewModel : BaseViewModel
    {
        private Gallery _gallery;
        private readonly ChannelFactory<IAuthor> _channelFactoryAuthor;
        private readonly ChannelFactory<IWorkOfArt> _channelFactoryWorkOfArt;

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

        public GalleryDetailsViewModel(Gallery gallery)
        {
            var bindingAuthor = new NetTcpBinding();
            var endpointAuthor = new EndpointAddress("net.tcp://localhost:8088/Author");
            _channelFactoryAuthor = new ChannelFactory<IAuthor>(bindingAuthor, endpointAuthor);

            var bindingWorkOfArt = new NetTcpBinding();
            var endpointWorkOfArt = new EndpointAddress("net.tcp://localhost:8087/WorkOfArt");
            _channelFactoryWorkOfArt = new ChannelFactory<IWorkOfArt>(bindingWorkOfArt, endpointWorkOfArt);

            Gallery = gallery;
            WorkOfArts = new ObservableCollection<WorkOfArt>(gallery.WorkOfArts);

            FetchAuthorNames();

            DetailsWorkOfArtCommand = new RelayCommand<WorkOfArt>(DetailsWorkOfArt);
            DeleteWorkOfArtCommand = new RelayCommand<WorkOfArt>(DeleteWorkOfArt);
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

        public ICommand DetailsWorkOfArtCommand { get; }
        public ICommand DeleteWorkOfArtCommand { get; }

        private void DetailsWorkOfArt(WorkOfArt workOfArt)
        {
            MessageBox.Show($"Viewing details for {workOfArt.ArtName}");
            // Implementacija logike za prikaz detalja umetničkog dela
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
