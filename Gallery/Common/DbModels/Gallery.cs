using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Common.DbModels
{
    [DataContract]
    public class Gallery
    {
        [Key]
        [DataMember]
        public string PIB { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string MBR { get; set; }

        [DataMember]
        public List<WorkOfArt> WorkOfArts { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

        [DataMember]
        public bool IsInEditingMode { get; set; }

        [DataMember]
        public string GalleryIsEdditedBy { get; set; } // username of user that edits gallery currently
    }
}
