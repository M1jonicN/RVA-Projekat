using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Common.DbModels
{
    public class Gallery
    {
        [Key]
        public string PIB { get; set; }
        public string Address { get; set; }
        public string MBR { get; set; }
        public List<WorkOfArt> WorkOfArts { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsInEditingMode { get; set; }
        public string GalleryIsEdditedBy { get; set; } // username of user that edits gallery currenly

    }
}
