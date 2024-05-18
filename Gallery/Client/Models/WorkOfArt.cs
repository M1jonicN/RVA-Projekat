using Client.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class WorkOfArt
    {
        private string artName;
        private ArtMovement artMovement;
        private Style style;
        private int authorID;
        private bool isDeleted;

        public string ArtName { get => artName; set => artName = value; }
        public ArtMovement ArtMovement { get => artMovement; set => artMovement = value; }
        public Style Style { get => style; set => style = value; }
        public int AuthorID { get => authorID; set => authorID = value; }
        public bool IsDeleted { get => isDeleted; set => isDeleted = value; }
    }
}
