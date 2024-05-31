using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Timers;
using System.Windows.Input;
using Client.Helpers;
using Client.Models;
using Client.Services;
using Common.DbModels;
using Common.Interfaces;

namespace Client.ViewModels
{
    public class WorkOfArtDetailsViewModel : BaseViewModel
    {
        #region Fields
        private Timer _timer;
        private Common.DbModels.WorkOfArt _workOfArt;
        private Common.DbModels.Author _author;
        private bool _isWorkOfArtEditing;
        private bool _isAuthorEditing;
        #endregion

        #region Properties
        public Common.DbModels.WorkOfArt WorkOfArt
        {
            get => _workOfArt;
            set
            {
                _workOfArt = value;
                OnPropertyChanged();
            }
        }

        public Common.DbModels.Author Author
        {
            get => _author;
            set
            {
                _author = value;
                OnPropertyChanged();
            }
        }

        private readonly Common.DbModels.User _loggedInUser;
        public string LoggedInUsername => _loggedInUser.Username;

        public bool IsWorkOfArtEditing
        {
            get => _isWorkOfArtEditing;
            set
            {
                _isWorkOfArtEditing = value;
                OnPropertyChanged();
            }
        }

        public bool IsAuthorEditing
        {
            get => _isAuthorEditing;
            set
            {
                _isAuthorEditing = value;
                OnPropertyChanged();
            }
        }
        public IEnumerable<ArtMovement> ArtMovements => Enum.GetValues(typeof(ArtMovement)).Cast<ArtMovement>();
        public IEnumerable<Style> Styles => Enum.GetValues(typeof(Style)).Cast<Style>();
        #endregion

        #region Commands
        public ICommand EditWorkOfArtCommand { get; }
        public ICommand SaveWorkOfArtCommand { get; }
        public ICommand EditAuthorCommand { get; }
        public ICommand SaveAuthorCommand { get; }
        public ICommand DeleteAuthorCommand { get; }

        #endregion
        public WorkOfArtDetailsViewModel(Common.DbModels.WorkOfArt workOfArt, Common.DbModels.Author author, Common.DbModels.User loggedInUser)
        {
            WorkOfArt = workOfArt;
            Author = author;
            _loggedInUser = loggedInUser;

            EditWorkOfArtCommand = new RelayCommand(EditWorkOfArt);
            SaveWorkOfArtCommand = new RelayCommand(SaveWorkOfArt);
            EditAuthorCommand = new RelayCommand(EditAuthor);
            SaveAuthorCommand = new RelayCommand(SaveAuthor);
            DeleteAuthorCommand = new RelayCommand(DeleteAuthor);

            _timer = new Timer(1000);
            _timer.Elapsed += (sender, args) => RefreshData();
            _timer.Start();
        }

        #region Methods
        private void EditWorkOfArt()
        {
            IsWorkOfArtEditing = true;
        }

        private void SaveWorkOfArt()
        {
            IsWorkOfArtEditing = false;
            var clientWorkOfArt = new ChannelFactory<IWorkOfArtService>(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:8087/WorkOfArt")).CreateChannel();
            clientWorkOfArt.UpdateWorkOfArt(WorkOfArt);
            RefreshWorkOfArt();
        }

        private void EditAuthor()
        {
            IsAuthorEditing = true;
        }

        private void SaveAuthor()
        {
            IsAuthorEditing = false;
            var clientAuthor = new ChannelFactory<IAuthorService>(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:8088/Author")).CreateChannel();
            clientAuthor.SaveAuthorChanges(Author);
            RefreshAuthor();
        }

        private void DeleteAuthor()
        {
            var clientAuthor = new ChannelFactory<IAuthorService>(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:8088/Author")).CreateChannel();
            var success = clientAuthor.DeleteAuhor(Author.ID);

            if (success)
            {
                var clientWoa = new ChannelFactory<IWorkOfArtService>(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:8087/WorkOfArt")).CreateChannel();
                clientWoa.GetAllWorkOfArtsDeletedForAuthorId(Author.ID);
                Console.WriteLine("Author deleted successfully.");
                UserActionLoggerService.Instance.Log(_loggedInUser.Username, $" author {Author.FirstName} {Author.LastName} deleted successfully.");
                Author = new Common.DbModels.Author(); // or null, depending on your logic
                OnPropertyChanged(nameof(Author));
            }
            else
            {
                Console.WriteLine("Failed to delete author.");
            }
        }

        private void RefreshData()
        {
            if (!IsWorkOfArtEditing)
            {
                RefreshWorkOfArt();
            }
            if (!IsAuthorEditing)
            {
                RefreshAuthor();
            }
        }

        private void RefreshWorkOfArt()
        {
            var clientWorkOfArt = new ChannelFactory<IWorkOfArtService>(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:8087/WorkOfArt")).CreateChannel();
            var updatedWorkOfArt = clientWorkOfArt.GetWorkOfArtById(WorkOfArt.ID);

            if (updatedWorkOfArt != null)
            {
                WorkOfArt.ArtName = updatedWorkOfArt.ArtName;
                WorkOfArt.ArtMovement = updatedWorkOfArt.ArtMovement;
                WorkOfArt.Style = updatedWorkOfArt.Style;
                WorkOfArt.GalleryPIB = updatedWorkOfArt.GalleryPIB;
                OnPropertyChanged(nameof(WorkOfArt));
                Console.WriteLine("Work of Art refreshed.");
            }
            else
            {
                Console.WriteLine("Failed to refresh Work of Art.");
            }
        }

        private void RefreshAuthor()
        {
            var clientAuthor = new ChannelFactory<IAuthorService>(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:8088/Author")).CreateChannel();
            var updatedAuthor = clientAuthor.GetAuthorById(Author.ID);

            if (updatedAuthor != null)
            {
                Author.FirstName = updatedAuthor.FirstName;
                Author.LastName = updatedAuthor.LastName;
                Author.BirthYear = updatedAuthor.BirthYear;
                Author.DeathYear = updatedAuthor.DeathYear;
                Author.ArtMovement = updatedAuthor.ArtMovement;
                OnPropertyChanged(nameof(Author));
                Console.WriteLine("Author refreshed.");
            }
            else
            {
                Console.WriteLine("Failed to refresh Author.");
            }
        }
        #endregion
    }
}
