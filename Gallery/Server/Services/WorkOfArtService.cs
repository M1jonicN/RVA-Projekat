using Common.DbModels;
using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services
{
    public class WorkOfArtService : IWorkOfArt
    {
        private readonly MyDbContext dbContext;
        public WorkOfArtService()
        {
            dbContext = new MyDbContext();
        }
        public List<WorkOfArt> GetWorkOfArtsForGallery(string galleryPib)
        {
            var workOfArtsForGallery = dbContext.WorkOfArts.Where(woa => woa.GalleryPIB.Equals(galleryPib) && !woa.IsDeleted);
            return workOfArtsForGallery.ToList();
        }
    }
}
