using System.ComponentModel.DataAnnotations;

namespace Common.DbModels
{
    public class WorkOfArt
    {
        [Key]
        public int ID { get; set; }
        public string ArtName { get; set; }
        public ArtMovement ArtMovement { get; set; }
        public Style Style { get; set; }
        public int AuthorID { get; set; }
        public string AuthorName { get; set; }
        public string GalleryPIB { get; set; }
        public bool IsDeleted { get; set; }
    }
}
