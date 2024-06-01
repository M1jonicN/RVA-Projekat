using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Common.DbModels
{
    [DataContract]
    public class WorkOfArt
    {
        [Key]
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string ArtName { get; set; }

        [EnumDataType(typeof(ArtMovement))]
        [DataMember]
        public ArtMovement ArtMovement { get; set; }

        [EnumDataType(typeof(Style))]
        [DataMember]
        public Style Style { get; set; }

        [DataMember]
        public int AuthorID { get; set; }

        [DataMember]
        public string AuthorName { get; set; }

        [DataMember]
        public string GalleryPIB { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }
    }
}
