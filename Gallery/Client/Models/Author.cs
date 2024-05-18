using Client.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class Author 
    {
        private int id;
        private string firstName;
        private string lastName;
        private int birthYear;
        private int deathYear;
        private ArtMovement artMovement;
        private bool isDeleted;

        public int Id { get => id; set => id = value; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string LastName { get => lastName; set => lastName = value; }
        public int BirthYear { get => birthYear; set => birthYear = value; }
        public int DeathYear { get => deathYear; set => deathYear = value; }
        public ArtMovement ArtMovement { get => artMovement; set => artMovement = value; }
        public bool IsDeleted { get => isDeleted; set => isDeleted = value; }
    }
}
