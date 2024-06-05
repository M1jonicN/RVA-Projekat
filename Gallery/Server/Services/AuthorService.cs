using Common.DbModels;
using Common.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Server.Services
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class AuthorService : IAuthorService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AuthorService));
        private static MyDbContext dbContext;

        public AuthorService()
        {
            dbContext = MyDbContext.Instance;
            log.Info("AuthorService instance created.");
        }

        #region Methods
        public bool CreateNewAuthor(Author newAuthor)
        {
            if (dbContext.Authors.Any(a => a.ID == newAuthor.ID))
            {
                log.Warn($"CreateNewAuthor failed: Author with ID {newAuthor.ID} already exists.");
                return false;
            }

            var author = new Author
            {
                FirstName = newAuthor.FirstName,
                LastName = newAuthor.LastName,
                BirthYear = newAuthor.BirthYear,
                DeathYear = newAuthor.DeathYear,
                ArtMovement = newAuthor.ArtMovement,
                IsDeleted = false
            };

            dbContext.Authors.Add(author);
            dbContext.SaveChanges();
            log.Info($"Author {newAuthor.FirstName} {newAuthor.LastName} created successfully.");
            return true;
        }

        public bool DeleteAuhor(int authorID)
        {
            var author = dbContext.Authors.FirstOrDefault(a => a.ID == authorID);

            if (author != null)
            {
                author.IsDeleted = true;
                dbContext.SaveChanges();
                log.Info($"Author with ID {authorID} marked as deleted.");
                return true;
            }

            log.Warn($"DeleteAuhor failed: Author with ID {authorID} not found.");
            return false;
        }

        public List<Author> GetAllAuthores()
        {
            var authores = dbContext.Authors.ToList();
            log.Info("GetAllAuthores called.");
            return authores;
        }

        public Author GetAuthorById(int authorId)
        {
            var author = dbContext.Authors.FirstOrDefault(a => a.ID == authorId && !a.IsDeleted);
            log.Info($"GetAuthorById called for ID {authorId}.");
            return author;
        }

        public Author GetAuthorByWorkOfArtId(int workOfArtId)
        {
            WorkOfArt workOfArt = dbContext.WorkOfArts.FirstOrDefault(woa => woa.ID == workOfArtId);
            if (workOfArt == null)
            {
                log.Warn($"GetAuthorByWorkOfArtId failed: Work of art with ID {workOfArtId} not found.");
                throw new FaultException("Work of art not found.");
            }

            Author author = dbContext.Authors.FirstOrDefault(a => a.ID == workOfArt.AuthorID);
            if (author == null)
            {
                log.Warn($"GetAuthorByWorkOfArtId failed: Author with ID {workOfArt.AuthorID} not found.");
                throw new FaultException("Author not found.");
            }

            log.Info($"GetAuthorByWorkOfArtId called for workOfArtId {workOfArtId}.");
            return author;
        }

        public string GetAuthorNameForWorkOfArt(int workOfArtId, string galleryPIB)
        {
            var workOfArt = dbContext.WorkOfArts.FirstOrDefault(woa => woa.ID == workOfArtId && woa.GalleryPIB == galleryPIB);
            if (workOfArt == null)
            {
                log.Warn($"GetAuthorNameForWorkOfArt failed: Work of art with ID {workOfArtId} not found.");
                throw new FaultException("Work of art not found.");
            }

            var author = dbContext.Authors.FirstOrDefault(a => a.ID == workOfArt.AuthorID);
            if (author == null)
            {
                log.Warn($"GetAuthorNameForWorkOfArt failed: Author with ID {workOfArt.AuthorID} not found.");
                throw new FaultException("Author not found.");
            }

            log.Info($"GetAuthorNameForWorkOfArt called for workOfArtId {workOfArtId} and galleryPIB {galleryPIB}.");
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
                    log.Info($"Author with ID {author.ID} updated successfully.");
                    return true;
                }

                log.Warn($"SaveAuthorChanges failed: Author with ID {author.ID} not found.");
                return false;
            }
            catch (Exception ex)
            {
                log.Error($"Error saving author: {ex.Message}", ex);
                return false;
            }
        }
        #endregion
    }
}
