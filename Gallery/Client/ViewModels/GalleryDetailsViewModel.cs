using Common.DbModels;
using Common.Interfaces;
using System.Collections.ObjectModel;
using System.ServiceModel;

namespace Client.ViewModels
{
    public class GalleryDetailsViewModel : BaseViewModel
    {
        private Gallery _gallery;
        private readonly ChannelFactory<IAuthor> _channelFactoryAuthor;

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

            Gallery = gallery;
            WorkOfArts = new ObservableCollection<WorkOfArt>(gallery.WorkOfArts);

            FetchAuthorNames();
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
    }
}
