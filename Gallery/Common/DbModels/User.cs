using Common.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common.DbModels
{
    public class User
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Username { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
        [Required]
        public UserType UserType { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string PasswordHash
        {
            get;
            set;
        }

        [Required]
        public bool IsLoggedIn { get; set; }


    }
}
