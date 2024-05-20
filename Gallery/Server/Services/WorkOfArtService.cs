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

        public bool UpdateWorkOfArt(WorkOfArt workOfArt)
        {
            try
            {
                var existingWoa = dbContext.WorkOfArts.FirstOrDefault(woa => woa.ID == workOfArt.ID);
                if (existingWoa != null)
                {
                    existingWoa.ArtName = workOfArt.ArtName;
                    existingWoa.ArtMovement = workOfArt.ArtMovement;
                    existingWoa.Style = workOfArt.Style;

                    dbContext.SaveChanges();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving Work Of Art: {ex.Message}");
                return false;
            }
        }

        public bool DeleteWorkOfArt(int workOfArtId)
        {
            var workOfArt = dbContext.WorkOfArts.FirstOrDefault(woa => woa.ID == workOfArtId);

            // Ako je galerija pronađena, obrišite je iz baze podataka
            if (workOfArt != null)
            {
                workOfArt.IsDeleted = true;
                dbContext.SaveChanges();
                return true; // Galerija je uspešno obrisana
            }

            return false; // Galerija nije pronađena
        }
    }
}
