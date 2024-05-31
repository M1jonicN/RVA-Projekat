using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;
using Client.Helpers;
using Client.Models;
using Client.Services;
using Common.DbModels;
using Common.Interfaces;

namespace Client.ViewModels
{
    public class CreateAuthorViewModel : BaseViewModel
    {
        #region Fields

        private string _firstName;
        private string _lastName;
        private int _birthYear;
        private int _deathYear;
        private ArtMovement _selectedArtMovement;
        private ObservableCollection<ArtMovement> _artMovements;
        private readonly ChannelFactory<IAuthorService> _channelFactoryAuthor;
        private string _loggedInUser;

        #endregion

        public CreateAuthorViewModel(string username)
        {
            _loggedInUser = username;
            ArtMovements = new ObservableCollection<ArtMovement>(Enum.GetValues(typeof(ArtMovement)) as IEnumerable<ArtMovement>);
            SaveCommand = new RelayCommand(Save);

            var bindingAuthor = new NetTcpBinding();
            var endpointAuthor = new EndpointAddress("net.tcp://localhost:8088/Author");
            _channelFactoryAuthor = new ChannelFactory<IAuthorService>(bindingAuthor, endpointAuthor);

        }
        public ICommand SaveCommand { get; }
        #region Properties
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        public int BirthYear
        {
            get => _birthYear;
            set => SetProperty(ref _birthYear, value);
        }

        public int DeathYear
        {
            get => _deathYear;
            set => SetProperty(ref _deathYear, value);
        }

        public ArtMovement SelectedArtMovement
        {
            get => _selectedArtMovement;
            set => SetProperty(ref _selectedArtMovement, value);
        }

        public ObservableCollection<ArtMovement> ArtMovements
        {
            get => _artMovements;
            set => SetProperty(ref _artMovements, value);
        }

        #endregion 

        #region Methods
        private void Save()
        {
            if (!CanCreateAuthor()) 
            {
                MessageBox.Show("Unsuccessfully create new Author!");
                UserActionLoggerService.Instance.Log(_loggedInUser, $" unsuccessfully created new Author.");
                return;
            }
            var clientAuthor = _channelFactoryAuthor.CreateChannel();
            Common.DbModels.Author newAuthor = new Common.DbModels.Author()
            {
                FirstName = FirstName,
                LastName = LastName,
                BirthYear = BirthYear,
                DeathYear = DeathYear,
                ArtMovement = SelectedArtMovement,
                IsDeleted = false
            };
            var success = clientAuthor.CreateNewAuthor(newAuthor);
            if (success)
            {
                MessageBox.Show("Successfully created new Author!");
                UserActionLoggerService.Instance.Log(_loggedInUser, $" successfully created new Author.");
                Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
            }
            else
            {
                MessageBox.Show("Unsuccessfully create new Author!");
                UserActionLoggerService.Instance.Log(_loggedInUser, $" unseccessfully created new Author.");
                Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
            }
        }

        private bool CanCreateAuthor()
        {
            return !string.IsNullOrWhiteSpace(BirthYear.ToString()) &&
                   !string.IsNullOrWhiteSpace(DeathYear.ToString()) &&
                   !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName);
        }
        #endregion
    }
}
