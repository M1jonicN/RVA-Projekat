using Common.DbModels;
using Common.Helpers;
using Common.Interfaces;
using Common.Services;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Server.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class GalleryService : IGalleryService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GalleryService));
        private static MyDbContext dbContext;

        public GalleryService(MyDbContext context)
        {
            dbContext = MyDbContext.SingletonInstance;
            log.Info("GalleryService instance created with context.");
        }

        public GalleryService()
        {
            dbContext = MyDbContext.SingletonInstance;
            log.Info("GalleryService instance created.");
        }

        public List<Gallery> GetAllGalleries()
        {
            var galleries = dbContext.Galleries.Where(g => !g.IsDeleted).ToList();
            log.Info("Retrieved all galleries.");
            return galleries;
        }

        public bool CreateNewGallery(Gallery newGallery)
        {
            if (dbContext.Galleries.Any(g => g.PIB == newGallery.PIB))
            {
                log.Warn($"CreateNewGallery failed: Gallery with PIB {newGallery.PIB} already exists.");
                return false;
            }
            var _allGalleries = dbContext.Galleries.ToList();
            var gallery = new Gallery
            {
                PIB = PibHelper.GenerateUniquePIB(_allGalleries),
                MBR = newGallery.MBR,
                Address = newGallery.Address,
                IsDeleted = false
            };
            dbContext.Galleries.Add(gallery);
            dbContext.SaveChanges();
            log.Info($"New gallery created with PIB {gallery.PIB}.");
            return true;
        }

        public bool DeleteGallery(string galleryPIB)
        {
            var gallery = dbContext.Galleries.FirstOrDefault(g => g.PIB == galleryPIB);

            if (gallery != null)
            {
                gallery.IsDeleted = true;
                dbContext.SaveChanges();
                log.Info($"Gallery with PIB {galleryPIB} marked as deleted.");
                return true;
            }

            log.Warn($"DeleteGallery failed: Gallery with PIB {galleryPIB} not found.");
            return false;
        }

        public bool SaveGalleryChanges(Gallery gallery)
        {
            try
            {
                var existingGallery = dbContext.Galleries.FirstOrDefault(g => g.PIB == gallery.PIB);
                if (existingGallery != null)
                {
                    existingGallery.PIB = gallery.PIB;
                    existingGallery.MBR = gallery.MBR;
                    existingGallery.Address = gallery.Address;

                    dbContext.SaveChanges();
                    log.Info($"Gallery with PIB {gallery.PIB} updated successfully.");
                    return true;
                }

                log.Warn($"SaveGalleryChanges failed: Gallery with PIB {gallery.PIB} not found.");
                return false;
            }
            catch (Exception ex)
            {
                log.Error($"Error saving gallery: {ex.Message}", ex);
                return false;
            }
        }

        public Gallery GetGalleryByPIB(string pib)
        {
            var gallery = dbContext.Galleries.FirstOrDefault(g => g.PIB == pib && !g.IsDeleted);
            log.Info($"Retrieved gallery by PIB {pib}.");
            return gallery;
        }

        public List<Gallery> GetAllGalleriesFromDb()
        {
            var galleries = dbContext.Galleries.ToList();
            log.Info("Retrieved all galleries from database.");
            return galleries;
        }
    }
}
