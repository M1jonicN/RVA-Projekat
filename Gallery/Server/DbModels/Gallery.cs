using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DbModels
{
    public class Gallery
    {
        [Key]
        public string PIB { get; set; }
        public string Address { get; set; }
        public string MBR { get; set; }
        public List<WorkOfArt> WorkOfArts { get; set; }
    }
}
