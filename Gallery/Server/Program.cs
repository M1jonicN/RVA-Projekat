using System;
using System.Linq;
using log4net;
using log4net.Config;
using Common.Helper;
using Common.DbModels;
using System.Collections.Generic;

namespace Server
{
    internal class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        private static MyDbContext dbContext;

        static void Main(string[] args)
        {
            // Inicijalizacija log4net iz App.config fajla
            XmlConfigurator.Configure();

            try
            {
                log.Info("Application is starting...");

                dbContext = MyDbContext.Instance;  // Koristi Singleton instancu
                InitializeDatabaseData();

                OpenCloseServices.Open();
                log.Info("All services are up!");

                Console.ReadLine();

                OpenCloseServices.Close();
                log.Info("All services are down!");

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                log.Error("An error occurred while running the program", ex);
                // Rukovanje izuzetkom prilikom pokretanja aplikacije
                Console.WriteLine($"An error occurred while running the program: {ex.Message}");
            }
        }

        // Metod koji inicijalizuje podatke prilikom prvog pokretanja programa
        private static void InitializeDatabaseData()
        {
            if (!(dbContext.Users.Count() == 0 &&
                  dbContext.Galleries.Count() == 0 &&
                  dbContext.Authors.Count() == 0 &&
                  dbContext.WorkOfArts.Count() == 0))
            {
                return;
            }

            log.Info("Initializing database data...");

            try
            {
                User admin = new User()
                {
                    FirstName = "Nemanja",
                    LastName = "Mijonic",
                    Username = "admin",
                    UserType = UserType.Admin,
                    PasswordHash = HashHelper.ConvertToHash("admin"),
                    IsDeleted = false,
                    IsLoggedIn = false,
                };

                dbContext.Users.Add(admin);
                dbContext.SaveChanges();
                log.Debug("Admin user added to the database.");

                Author author1 = new Author()
                {
                    FirstName = "Leonardo",
                    LastName = "da Vinci",
                    BirthYear = 1452,
                    DeathYear = 1519,
                    ArtMovement = ArtMovement.Renaissance,
                    IsDeleted = false
                };

                Author author2 = new Author()
                {
                    FirstName = "Vincent",
                    LastName = "van Gogh",
                    BirthYear = 1853,
                    DeathYear = 1890,
                    ArtMovement = ArtMovement.PostImpressionism,
                    IsDeleted = false
                };

                Author author3 = new Author()
                {
                    FirstName = "Pablo",
                    LastName = "Picasso",
                    BirthYear = 1881,
                    DeathYear = 1973,
                    ArtMovement = ArtMovement.Cubism,
                    IsDeleted = false
                };

                dbContext.Authors.Add(author1);
                dbContext.Authors.Add(author2);
                dbContext.Authors.Add(author3);
                dbContext.SaveChanges();
                log.Debug("Authors added to the database.");

                Gallery gallery = new Gallery()
                {
                    PIB = "123456789",
                    Address = "123 Gallery Street, City, Country",
                    MBR = "987654321",
                    IsDeleted = false,
                    IsInEditingMode = false,
                    GalleryIsEdditedBy = ""
                };

                dbContext.Galleries.Add(gallery);
                dbContext.SaveChanges();
                log.Debug("Gallery added to the database.");

                WorkOfArt art1 = new WorkOfArt()
                {
                    ArtName = "Mona Lisa",
                    ArtMovement = ArtMovement.Renaissance,
                    Style = Style.Realism,
                    GalleryPIB = gallery.PIB,
                    AuthorID = author1.ID,
                    AuthorName = $"{author1.FirstName} {author1.LastName}",
                    IsDeleted = false
                };

                WorkOfArt art2 = new WorkOfArt()
                {
                    ArtName = "Starry Night",
                    ArtMovement = ArtMovement.PostImpressionism,
                    Style = Style.Expressionism,
                    GalleryPIB = gallery.PIB,
                    AuthorID = author2.ID,
                    AuthorName = $"{author2.FirstName} {author2.LastName}",
                    IsDeleted = false
                };

                WorkOfArt art3 = new WorkOfArt()
                {
                    ArtName = "Guernica",
                    ArtMovement = ArtMovement.Cubism,
                    Style = Style.Surrealism,
                    GalleryPIB = gallery.PIB,
                    AuthorID = author3.ID,
                    AuthorName = $"{author3.FirstName} {author3.LastName}",
                    IsDeleted = false
                };

                dbContext.WorkOfArts.Add(art1);
                dbContext.WorkOfArts.Add(art2);
                dbContext.WorkOfArts.Add(art3);
                dbContext.SaveChanges();
                log.Debug("Works of art added to the database.");

                // Dodela WorkOfArts galeriji
                gallery.WorkOfArts = new List<WorkOfArt>() { art1, art2, art3 };
                dbContext.SaveChanges();
                log.Debug("Works of art assigned to the gallery.");
            }
            catch (Exception ex)
            {
                log.Error("An error occurred while initializing the database data", ex);
                throw;
            }
        }
    }
}
