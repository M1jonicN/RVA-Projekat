using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class Gallery : INotifyPropertyChanged
    {
        private string pib;
        private string address;
        private string mbr;
        private List<WorkOfArt> workOfArts;

        public string PIB
        {
            get { return pib; }
            set
            {
                pib = value;
                OnPropertyChanged("PIB");
            }
        }

        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                OnPropertyChanged("Adresa");
            }
        }

        public string MBR
        {
            get { return mbr; }
            set
            {
                mbr = value;
                OnPropertyChanged("MBR");
            }
        }

        public List<WorkOfArt> WorkOfArts
        {
            get { return workOfArts; }
            set
            {
                workOfArts = value;
                OnPropertyChanged("WorkOfArts");
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
