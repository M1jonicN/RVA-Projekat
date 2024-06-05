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
                log.Info("Database is already initialized...");
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

                Author author4 = new Author()
                {
                    FirstName = "Claude",
                    LastName = "Monet",
                    BirthYear = 1840,
                    DeathYear = 1926,
                    ArtMovement = ArtMovement.Impressionism,
                    IsDeleted = false
                };

                Author author5 = new Author()
                {
                    FirstName = "Salvador",
                    LastName = "Dali",
                    BirthYear = 1904,
                    DeathYear = 1989,
                    ArtMovement = ArtMovement.Painting,
                    IsDeleted = false
                };

                dbContext.Authors.Add(author1);
                dbContext.Authors.Add(author2);
                dbContext.Authors.Add(author3);
                dbContext.Authors.Add(author4);
                dbContext.Authors.Add(author5);
                dbContext.SaveChanges();
                log.Debug("Authors added to the database.");

                Gallery gallery1 = new Gallery()
                {
                    PIB = "123456789",
                    Address = "123 Gallery Street, City, Country",
                    MBR = "987654321",
                    IsDeleted = false,
                    IsInEditingMode = false,
                    GalleryIsEdditedBy = ""
                };

                Gallery gallery2 = new Gallery()
                {
                    PIB = "987654321",
                    Address = "456 Art Avenue, Metropolis, Country",
                    MBR = "123456789",
                    IsDeleted = false,
                    IsInEditingMode = false,
                    GalleryIsEdditedBy = ""
                };

                dbContext.Galleries.Add(gallery1);
                dbContext.Galleries.Add(gallery2);
                dbContext.SaveChanges();
                log.Debug("Galleries added to the database.");

                WorkOfArt art1 = new WorkOfArt()
                {
                    ArtName = "Mona Lisa",
                    ArtMovement = ArtMovement.Renaissance,
                    Style = Style.Realism,
                    GalleryPIB = gallery1.PIB,
                    AuthorID = author1.ID,
                    AuthorName = $"{author1.FirstName} {author1.LastName}",
                    IsDeleted = false
                };

                WorkOfArt art2 = new WorkOfArt()
                {
                    ArtName = "Starry Night",
                    ArtMovement = ArtMovement.PostImpressionism,
                    Style = Style.Expressionism,
                    GalleryPIB = gallery1.PIB,
                    AuthorID = author2.ID,
                    AuthorName = $"{author2.FirstName} {author2.LastName}",
                    IsDeleted = false
                };

                WorkOfArt art3 = new WorkOfArt()
                {
                    ArtName = "Guernica",
                    ArtMovement = ArtMovement.Cubism,
                    Style = Style.Surrealism,
                    GalleryPIB = gallery1.PIB,
                    AuthorID = author3.ID,
                    AuthorName = $"{author3.FirstName} {author3.LastName}",
                    IsDeleted = false
                };

                WorkOfArt art4 = new WorkOfArt()
                {
                    ArtName = "Water Lilies",
                    ArtMovement = ArtMovement.Impressionism,
                    Style = Style.Naturalism,
                    GalleryPIB = gallery2.PIB,
                    AuthorID = author4.ID,
                    AuthorName = $"{author4.FirstName} {author4.LastName}",
                    IsDeleted = false
                };

                WorkOfArt art5 = new WorkOfArt()
                {
                    ArtName = "The Persistence of Memory",
                    ArtMovement = ArtMovement.Baroque,
                    Style = Style.Surrealism,
                    GalleryPIB = gallery2.PIB,
                    AuthorID = author5.ID,
                    AuthorName = $"{author5.FirstName} {author5.LastName}",
                    IsDeleted = false
                };

                dbContext.WorkOfArts.Add(art1);
                dbContext.WorkOfArts.Add(art2);
                dbContext.WorkOfArts.Add(art3);
                dbContext.WorkOfArts.Add(art4);
                dbContext.WorkOfArts.Add(art5);
                dbContext.SaveChanges();
                log.Debug("Works of art added to the database.");

                // Dodela WorkOfArts galerijama
                gallery1.WorkOfArts = new List<WorkOfArt>() { art1, art2, art3 };
                gallery2.WorkOfArts = new List<WorkOfArt>() { art4, art5 };
                dbContext.SaveChanges();
                log.Debug("Works of art assigned to the galleries.");
            }
            catch (Exception ex)
            {
                log.Error("An error occurred while initializing the database data", ex);
                throw;
            }
        }

    }
}
