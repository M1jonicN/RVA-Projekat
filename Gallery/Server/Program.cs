using System;
using System.Collections.Generic;
using System.Linq;
using Common.Helper;
using Common.DbModels;

namespace Server
{
    internal class Program
    {
        private static MyDbContext dbContext;

        static void Main(string[] args)
        {
            try
            {
                string has = HashHelper.ConvertToHash("filip");
                Console.WriteLine(has);

                dbContext = new MyDbContext();
                InitializeDatabaseData();

                OpenCloseServices.Open();
                Console.WriteLine("All services are up!");

                Console.ReadLine();

                OpenCloseServices.Close();
                Console.WriteLine("All services are down!");

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                // Rukovanje izuzetkom prilikom pokretanja aplikacije
                Console.WriteLine($"An error occurred while running the program: {ex.Message}");
            }
        }

        // Method that initializes data for the first time running the program
        private static void InitializeDatabaseData()
        {
            if (!(dbContext.Users.Count() == 0 &&
                  dbContext.Galleries.Count() == 0 &&
                  dbContext.Authors.Count() == 0 &&
                  dbContext.WorkOfArts.Count() == 0))
            {
                return;
            }

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

            Gallery gallery = new Gallery()
            {
                PIB = "123456789",
                Address = "123 Gallery Street, City, Country",
                MBR = "987654321",
                IsDeleted = false
            };

            dbContext.Galleries.Add(gallery);
            dbContext.SaveChanges();

            WorkOfArt art1 = new WorkOfArt()
            {
                ArtName = "Mona Lisa",
                ArtMovement = ArtMovement.Renaissance,
                Style = Style.Realism,
                GalleryPIB = gallery.PIB,
                AuthorID = author1.ID,
                AuthorName = $"{author1.FirstName} {author1.LastName}", // Dodavanje imena autora
                IsDeleted = false
            };

            WorkOfArt art2 = new WorkOfArt()
            {
                ArtName = "Starry Night",
                ArtMovement = ArtMovement.PostImpressionism,
                Style = Style.Expressionism,
                GalleryPIB = gallery.PIB,
                AuthorID = author2.ID,
                AuthorName = $"{author2.FirstName} {author2.LastName}", // Dodavanje imena autora
                IsDeleted = false
            };

            WorkOfArt art3 = new WorkOfArt()
            {
                ArtName = "Guernica",
                ArtMovement = ArtMovement.Cubism,
                Style = Style.Surrealism,
                GalleryPIB = gallery.PIB,
                AuthorID = author3.ID,
                AuthorName = $"{author3.FirstName} {author3.LastName}", // Dodavanje imena autora
                IsDeleted = false
            };

            dbContext.WorkOfArts.Add(art1);
            dbContext.WorkOfArts.Add(art2);
            dbContext.WorkOfArts.Add(art3);
            dbContext.SaveChanges();

            // Assign WorkOfArts to Gallery
            gallery.WorkOfArts = new List<WorkOfArt>() { art1, art2, art3 };
            dbContext.SaveChanges();
        }
    }
}
