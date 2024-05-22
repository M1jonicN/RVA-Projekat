using Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Common.DbModels;
using System.ServiceModel;

namespace Server.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class GalleryService : IGalleryService
    {
        private readonly MyDbContext dbContext;
        private readonly IMapper mapper;

        public GalleryService(MyDbContext context, IMapper mapper)
        {
            this.dbContext = context;
            this.mapper = mapper;
        }

        public GalleryService()
        {
            this.dbContext = new MyDbContext();
        }

        public List<Gallery> GetAllGalleries()
        {
            var galleries = dbContext.Galleries.Where(g=> !g.IsDeleted).ToList();
            return galleries;
        }

        public bool CreateNewGallery(Gallery newGallery)
        {
            if (dbContext.Galleries.Any(g => g.PIB == newGallery.PIB))
                return false;

            var gallery = new Gallery {
                PIB = newGallery.PIB,
                MBR = newGallery.MBR,
                Address = newGallery.Address,
                IsDeleted = false
            };
            dbContext.Galleries.Add(gallery);
            dbContext.SaveChanges();
            return true;
        }

        public bool DeleteGallery(string galleryPIB)
        {
            var gallery = dbContext.Galleries.FirstOrDefault(g => g.PIB == galleryPIB);

            // Ako je galerija pronađena, obrišite je iz baze podataka
            if (gallery != null)
            {
                gallery.IsDeleted = true;
                dbContext.SaveChanges();
                return true; // Galerija je uspešno obrisana
            }

            return false; // Galerija nije pronađena
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
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving gallery: {ex.Message}");
                return false;
            }
        }

        public Gallery GetGalleryByPIB(string pib) // Implementacija nove metode
        {
            return dbContext.Galleries.FirstOrDefault(g => g.PIB == pib && !g.IsDeleted);
        }

        public List<Gallery> GetAllGalleriesFromDb()
        {
            return dbContext.Galleries.ToList();
        }
    }
}
