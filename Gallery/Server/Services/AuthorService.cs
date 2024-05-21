using System.ServiceModel;
using Common.DbModels;
using Common.Interfaces;
using System.Linq;
using System;

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

        public bool DeleteAuhor(int authorID)
        {
            var author = dbContext.Authors.FirstOrDefault(a => a.ID == authorID);

            if (author != null)
            {
                author.IsDeleted = true;
                dbContext.SaveChanges();
                return true; 
            }

            return false;
        }

        public Author GetAuthorById(int authorId)
        {
            return dbContext.Authors.FirstOrDefault(a => a.ID == authorId && !a.IsDeleted);
        }

        public Author GetAuthorByWorkOfArtId(int workOfArtId)
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

        public bool SaveAuthorChanges(Author author)
        {
            try
            {
                var existingAuthor = dbContext.Authors.FirstOrDefault(a => a.ID == author.ID);
                if (existingAuthor != null)
                {
                    existingAuthor.FirstName = author.FirstName;
                    existingAuthor.LastName = author.LastName;
                    existingAuthor.BirthYear = author.BirthYear;
                    existingAuthor.DeathYear = author.DeathYear;
                    existingAuthor.ArtMovement = author.ArtMovement;

                    dbContext.SaveChanges();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving author: {ex.Message}");
                return false;
            }
        }
    }
}
