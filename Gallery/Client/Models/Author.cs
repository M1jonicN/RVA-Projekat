using Client.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class Author : INotifyPropertyChanged
    {
        private string firstName;
        private string lastName;
        private int birthYear;
        private int deathYear;
        private ArtMovement artMovement;

        public string FirstName {
            get
            {
                return firstName;
            }
            set
            {
                firstName = value;
                OnPropertyChanged("FirstName");
            }
        }
        public string LastName
        {
            get
            {
                return lastName;
            }
            set
            {
                lastName = value;
                OnPropertyChanged("LastName");
            }
        }
        public int BirthYear {
            get
            {
                return birthYear;
            }
            set
            {
                birthYear = value;
                OnPropertyChanged("BirthYear");
            }
        }
        public int DeathYear {
            get
            {
                return deathYear;
            }
            set
            {
                deathYear = value;
                OnPropertyChanged("DeathYear");
            }
        }
        public ArtMovement ArtMovement {
            get
            {
                return artMovement;
            }
            set
            {
                artMovement = value;
                OnPropertyChanged("ArtMovement");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
