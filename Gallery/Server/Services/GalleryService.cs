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
            var galleries = dbContext.Galleries.ToList();
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
    }
}
