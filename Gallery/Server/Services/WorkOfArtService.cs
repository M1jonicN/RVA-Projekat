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
        private static MyDbContext dbContext;
        public WorkOfArtService()
        {
            dbContext = MyDbContext.SingletonInstance;
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

        public WorkOfArt GetWorkOfArtById(int workOfArtId)
        {
            return dbContext.WorkOfArts.FirstOrDefault(woa => woa.ID == workOfArtId && !woa.IsDeleted);
        }

        public List<WorkOfArt> GetAllWorkOfArts()
        {
            return dbContext.WorkOfArts.ToList();
        }

        public bool CreateNewWorkOfArt(WorkOfArt newWorkOfArt)
        {
            if (dbContext.WorkOfArts.Any(wa => wa.ID == newWorkOfArt.ID))
                return false;

            var woa = new WorkOfArt
            {
                ArtName = newWorkOfArt.ArtName,
                ArtMovement = newWorkOfArt.ArtMovement,
                Style = newWorkOfArt.Style,
                AuthorID = newWorkOfArt.AuthorID,
                AuthorName = newWorkOfArt.AuthorName,
                GalleryPIB = newWorkOfArt.GalleryPIB,
                IsDeleted = false
            };
            dbContext.WorkOfArts.Add(woa);
            dbContext.SaveChanges();
            return true;
        }

        public void GetAllWorkOfArtsDeletedForAuthorId(int authorID)
        {
            var workOfArts = dbContext.WorkOfArts.ToList();
            foreach (var woa in workOfArts) 
            {
                if (woa.AuthorID == authorID) 
                {
                    woa.IsDeleted = true;
                }
            }
            dbContext.SaveChanges();
        }
        
    }
}
