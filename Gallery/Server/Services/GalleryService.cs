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

        public List<Author> GetAllAuthores()
        {
            var authors = dbContext.Authors.ToList();
            return authors;
        }

        public List<Gallery> GetAllGalleries()
        {
            var galleries = dbContext.Galleries.ToList();
            return mapper.Map<List<Gallery>>(galleries);
        }

        public List<WorkOfArt> GetAllWorkOfArts()
        {
            var worksOfArt = dbContext.WorkOfArts.ToList();
            return mapper.Map<List<WorkOfArt>>(worksOfArt);
        }


    }
}
