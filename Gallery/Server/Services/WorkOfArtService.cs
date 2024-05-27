using Common.DbModels;
using Common.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Services
{
    public class WorkOfArtService : IWorkOfArt
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WorkOfArtService));
        private static MyDbContext dbContext;

        public WorkOfArtService()
        {
            dbContext = MyDbContext.SingletonInstance;
            log.Info("WorkOfArtService instance created.");
        }

        public List<WorkOfArt> GetWorkOfArtsForGallery(string galleryPib)
        {
            var workOfArtsForGallery = dbContext.WorkOfArts.Where(woa => woa.GalleryPIB.Equals(galleryPib) && !woa.IsDeleted);
            log.Info($"Retrieved works of art for gallery PIB: {galleryPib}");
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
                    log.Info($"Work of art ID {workOfArt.ID} updated successfully.");
                    return true;
                }

                log.Warn($"Update failed: Work of art ID {workOfArt.ID} not found.");
                return false;
            }
            catch (Exception ex)
            {
                log.Error($"Error updating work of art: {ex.Message}", ex);
                return false;
            }
        }

        public bool DeleteWorkOfArt(int workOfArtId)
        {
            var workOfArt = dbContext.WorkOfArts.FirstOrDefault(woa => woa.ID == workOfArtId);

            if (workOfArt != null)
            {
                workOfArt.IsDeleted = true;
                dbContext.SaveChanges();
                log.Info($"Work of art ID {workOfArtId} marked as deleted.");
                return true;
            }

            log.Warn($"Delete failed: Work of art ID {workOfArtId} not found.");
            return false;
        }

        public WorkOfArt GetWorkOfArtById(int workOfArtId)
        {
            var workOfArt = dbContext.WorkOfArts.FirstOrDefault(woa => woa.ID == workOfArtId && !woa.IsDeleted);
            log.Info($"Retrieved work of art by ID {workOfArtId}.");
            return workOfArt;
        }

        public List<WorkOfArt> GetAllWorkOfArts()
        {
            var worksOfArt = dbContext.WorkOfArts.ToList();
            log.Info("Retrieved all works of art.");
            return worksOfArt;
        }

        public bool CreateNewWorkOfArt(WorkOfArt newWorkOfArt)
        {
            if (dbContext.WorkOfArts.Any(wa => wa.ID == newWorkOfArt.ID))
            {
                log.Warn($"CreateNewWorkOfArt failed: Work of art with ID {newWorkOfArt.ID} already exists.");
                return false;
            }

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
            log.Info($"New work of art {newWorkOfArt.ArtName} created successfully.");
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
            log.Info($"Marked all works of art as deleted for author ID {authorID}.");
        }
    }
}
