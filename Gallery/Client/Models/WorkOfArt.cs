using Client.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class WorkOfArt : INotifyPropertyChanged
    {
        private string artName;
        private ArtMovement artMovement;
        private Style style;
        private int authorID;

        public string ArtName {
            get
            {
                return artName;
            }
            set
            {
                artName = value;
                OnPropertyChanged("ArtName");
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
        public Style Style {
            get
            {
                return style;
            }
            set
            {
                style = value;
                OnPropertyChanged("Style");
            }
        }
        public int AuthorID
        {
            get
            {
                return authorID;
            }
            set
            {
                authorID = value;
                OnPropertyChanged("AuthorID");
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
