using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Common.DbModels
{
    [DataContract]
    public class Author
    {
        [Key]
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public int BirthYear { get; set; }

        [DataMember]
        public int DeathYear { get; set; }

        [EnumDataType(typeof(ArtMovement))]
        [DataMember]
        public ArtMovement ArtMovement { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }
    }
}
