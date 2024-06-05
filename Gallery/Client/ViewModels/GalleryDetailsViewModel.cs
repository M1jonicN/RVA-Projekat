using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Client.Commands;
using Client.Helpers;
using Client.Services;
using Client.Views;
using Common.DbModels;
using Common.Interfaces;
using Common.Services;
using log4net;
using Server.Services;

namespace Client.ViewModels
{
    public class GalleryDetailsViewModel : BaseViewModel
    {
        #region Fields
        private static readonly ILog log = LogManager.GetLogger(typeof(GalleryDetailsViewModel));
        private Gallery _gallery;
        private bool _isEditing;
        private readonly ChannelFactory<IAuthorService> _channelFactoryAuthor;
        private readonly ChannelFactory<IWorkOfArtService> _channelFactoryWorkOfArt;
        private readonly ChannelFactory<IGalleryService> _channelFactoryGallery;
        private readonly User _loggedInUser;
        private readonly DispatcherTimer _dispatcherTimer;

        private Gallery oldGallery;
        #endregion

        #region Properties
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
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public GalleryDetailsViewModel(Gallery gallery, User loggedInUser)
        {
            _loggedInUser = loggedInUser;

            var bindingAuthor = new NetTcpBinding();
            var endpointAuthor = new EndpointAddress("net.tcp://localhost:8088/Author");
            _channelFactoryAuthor = new ChannelFactory<IAuthorService>(bindingAuthor, endpointAuthor);

            var bindingWorkOfArt = new NetTcpBinding();
            var endpointWorkOfArt = new EndpointAddress("net.tcp://localhost:8087/WorkOfArt");
            _channelFactoryWorkOfArt = new ChannelFactory<IWorkOfArtService>(bindingWorkOfArt, endpointWorkOfArt);

            var bindingGallery = new NetTcpBinding();
            var endpointGallery = new EndpointAddress("net.tcp://localhost:8086/Gallery");
            _channelFactoryGallery = new ChannelFactory<IGalleryService>(bindingGallery, endpointGallery);

            Gallery = gallery;
            WorkOfArts = new ObservableCollection<WorkOfArt>(gallery.WorkOfArts);

            FetchAuthorNames();

            EditCommand = new RelayCommand(Edit);
            SaveCommand = new RelayCommand(Save);
            UndoCommand = new RelayCommand(Undo);
            RedoCommand = new RelayCommand(Redo);
            DetailsWorkOfArtCommand = new RelayCommand<WorkOfArt>(DetailsWorkOfArt);
            DeleteWorkOfArtCommand = new RelayCommand<WorkOfArt>(DeleteWorkOfArt);

            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            _dispatcherTimer.Tick += (sender, args) => RefreshGallery();
            _dispatcherTimer.Start();

            log.Info("GalleryDetailsViewModel initialized.");
            UserActionLoggerService.Instance.Log(_loggedInUser.Username, " initialized GalleryDetailsViewModel.");
        }

        #region Commands
        public ICommand DetailsWorkOfArtCommand { get; }
        public ICommand DeleteWorkOfArtCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        #endregion

        #region Methods
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
                log.Error("Failed to refresh gallery.", ex);
                MessageBox.Show($"Failed to refresh gallery: {ex.Message}");
            }
        }

        private void Edit()
        {

            if (!Gallery.IsInEditingMode)
            {
                oldGallery = new Gallery()
                {
                    MBR = Gallery.MBR,
                    Address = Gallery.Address,
                    IsDeleted = Gallery.IsDeleted,
                    PIB = Gallery.PIB,
                    
                };
                IsEditing = true;
                Gallery.IsInEditingMode = true;
                Gallery.GalleryIsEdditedBy = _loggedInUser.Username;
                var clientGallery = _channelFactoryGallery.CreateChannel();
                clientGallery.SaveGalleryChanges(Gallery);

                log.Info("Edit mode enabled.");
                UserActionLoggerService.Instance.Log(_loggedInUser.Username, " successfully enabled edit.");
            }
            else 
            {
                UserActionLoggerService.Instance.Log(_loggedInUser.Username, $" unsuccessfully enabled editing for Gallery PIB: {Gallery.PIB} becouse gallery is already being edited by {Gallery.GalleryIsEdditedBy}.");
                log.Info($"{_loggedInUser.Username} unsuccessfully enabled editing for Gallery PIB: {Gallery.PIB} becouse gallery is already being edited by {Gallery.GalleryIsEdditedBy}.");
            }
        }

        private void Save()
        {
            IsEditing = false;

            var clientGallery = _channelFactoryGallery.CreateChannel();
            UserActionLoggerService.Instance.Log(_loggedInUser.Username, " successfully saved changes.");
            Gallery.IsInEditingMode = false;
            Gallery.GalleryIsEdditedBy = "";
          //  clientGallery.SaveGalleryChanges(Gallery);

            var editGalleryCommand = new EditGalleryCommand(Gallery, clientGallery, oldGallery);
            Commands.CommandManager.ExecuteCommand(editGalleryCommand);
            

            log.Info("Changes saved.");
        }

        private void DetailsWorkOfArt(WorkOfArt workOfArt)
        {
            try
            {
                var clientAuthor = _channelFactoryAuthor.CreateChannel();
                var author = clientAuthor.GetAuthorByWorkOfArtId(workOfArt.ID);
                var detailsViewModel = new WorkOfArtDetailsViewModel(workOfArt, author, _loggedInUser);
                var detailsWindow = new WorkOfArtDetailsWindow()
                {
                    DataContext = detailsViewModel
                };
                detailsWindow.Show();

                log.Info($"Details for Work of Art {workOfArt.ArtName} opened.");
            }
            catch (Exception ex)
            {
                log.Error($"Failed to open details for Work of Art {workOfArt.ArtName}.", ex);
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

                log.Info($"Deleted Work of Art {workOfArt.ArtName}.");
                UserActionLoggerService.Instance.Log(_loggedInUser.Username, $" successfully deleted Work of Art with name: {workOfArt.ArtName}.");
            }
            catch (Exception ex)
            {
                log.Error($"Failed to delete Work of Art {workOfArt.ArtName}.", ex);
                MessageBox.Show($"Failed to delete {workOfArt.ArtName}: {ex.Message}");
                UserActionLoggerService.Instance.Log(_loggedInUser.Username, $" failed to delete Work of Art with name: {workOfArt.ArtName}.");
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


        #endregion
    }
}
