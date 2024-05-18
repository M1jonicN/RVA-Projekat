using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class Gallery 
    {
        private string pib;
        private string address;
        private string mbr;
        private List<WorkOfArt> workOfArts;
        private bool isDeleted;

        public string Pib { get => pib; set => pib = value; }
        public string Address { get => address; set => address = value; }
        public string Mbr { get => mbr; set => mbr = value; }
        public List<WorkOfArt> WorkOfArts { get => workOfArts; set => workOfArts = value; }
        public bool IsDeleted { get => isDeleted; set => isDeleted = value; }
    }
}
