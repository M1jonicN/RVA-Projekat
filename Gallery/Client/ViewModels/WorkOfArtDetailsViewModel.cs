using Common.DbModels;
using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Timers;
using System.Windows.Input;
using Client.Helpers;

namespace Client.ViewModels
{
    public class WorkOfArtDetailsViewModel : BaseViewModel
    {
        private Timer _timer;
        private WorkOfArt _workOfArt;
        private Author _author;
        private bool _isWorkOfArtEditing;
        private bool _isAuthorEditing;

        public WorkOfArt WorkOfArt
        {
            get => _workOfArt;
            set
            {
                _workOfArt = value;
                OnPropertyChanged();
            }
        }

        public Author Author
        {
            get => _author;
            set
            {
                _author = value;
                OnPropertyChanged();
            }
        }

        private readonly User _loggedInUser;
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

        public ICommand EditWorkOfArtCommand { get; }
        public ICommand SaveWorkOfArtCommand { get; }
        public ICommand EditAuthorCommand { get; }
        public ICommand SaveAuthorCommand { get; }

        public WorkOfArtDetailsViewModel(WorkOfArt workOfArt, Author author, User loggedInUser)
        {
            WorkOfArt = workOfArt;
            Author = author;
            _loggedInUser = loggedInUser;

            EditWorkOfArtCommand = new RelayCommand(EditWorkOfArt);
            SaveWorkOfArtCommand = new RelayCommand(SaveWorkOfArt);
            EditAuthorCommand = new RelayCommand(EditAuthor);
            SaveAuthorCommand = new RelayCommand(SaveAuthor);

            _timer = new Timer(2000);
            _timer.Elapsed += (sender, args) => RefreshData();
            _timer.Start();
        }

        private void EditWorkOfArt()
        {
            IsWorkOfArtEditing = true;
        }

        private void SaveWorkOfArt()
        {
            IsWorkOfArtEditing = false;
            var clientWorkOfArt = new ChannelFactory<IWorkOfArt>(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:8087/WorkOfArt")).CreateChannel();
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
            var clientAuthor = new ChannelFactory<IAuthor>(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:8088/Author")).CreateChannel();
            clientAuthor.SaveAuthorChanges(Author);
            RefreshAuthor();
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
            var clientWorkOfArt = new ChannelFactory<IWorkOfArt>(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:8087/WorkOfArt")).CreateChannel();
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
            var clientAuthor = new ChannelFactory<IAuthor>(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:8088/Author")).CreateChannel();
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
    }
}
