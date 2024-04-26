using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DbModels
{
    public class WorkOfArt
    {
        [Key]
        public int ID { get; set; }
        public string ArtName { get; set; }
        public ArtMovement ArtMovement { get; set; }
        public Style Style { get; set; }
        public int AuthorID { get; set; }
    }
}
