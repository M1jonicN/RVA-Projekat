using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Common.DbModels
{
    [DataContract]
    public class User
    {
        [Key]
        [DataMember]
        public int ID { get; set; }

        [Required]
        [DataMember]
        public string FirstName { get; set; }

        [Required]
        [DataMember]
        public string LastName { get; set; }

        [Required]
        [DataMember]
        public string Username { get; set; }

        [Required]
        [DataMember]
        public bool IsDeleted { get; set; }

        [EnumDataType(typeof(UserType))]
        [DataMember]
        public UserType UserType { get; set; }

        [Required]
        [DataMember]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }

        [Required]
        [DataMember]
        public bool IsLoggedIn { get; set; }
    }
}
