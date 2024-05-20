using System.ServiceModel;
using Common.DbModels;
using Common.Interfaces;
using System.Linq;

namespace Server.Services
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class AuthorService : IAuthor
    {
        private readonly MyDbContext dbContext;

        public AuthorService()
        {
            dbContext = new MyDbContext();
        }

        public Author GetAuthorById(int workOfArtId)
        {
            WorkOfArt workOfArt = dbContext.WorkOfArts.FirstOrDefault(woa => woa.ID == workOfArtId);
            if (workOfArt == null)
            {
                throw new FaultException("Work of art not found.");
            }

            Author author = dbContext.Authors.FirstOrDefault(a => a.ID == workOfArt.AuthorID);
            if (author == null)
            {
                throw new FaultException("Author not found.");
            }

            return author;
        }


        public string GetAuthorNameForWorkOfArt(int workOfArtId, string galleryPIB)
        {
            var workOfArt = dbContext.WorkOfArts.FirstOrDefault(woa => woa.ID == workOfArtId && woa.GalleryPIB == galleryPIB);
            if (workOfArt == null)
            {
                throw new FaultException("Work of art not found.");
            }

            var author = dbContext.Authors.FirstOrDefault(a => a.ID == workOfArt.AuthorID);
            if (author == null)
            {
                throw new FaultException("Author not found.");
            }

            return $"{author.FirstName} {author.LastName}";
        }
    }
}
