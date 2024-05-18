using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class User
    {
        private string firstName;
        private string lastName;
        private string username;
        private bool isDeleted;
        private UserType userType;

        public string FirstName { get => firstName; set => firstName = value; }
        public string LastName { get => lastName; set => lastName = value; }
        public string Username { get => username; set => username = value; }
        public bool IsDeleted { get => isDeleted; set => isDeleted = value; }
        public UserType UserType { get => userType; set => userType = value; }
    }
}
