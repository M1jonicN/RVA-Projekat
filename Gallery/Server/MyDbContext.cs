using Common.DbModels;
using System;
using System.Data.Entity;

namespace Server
{
    public class MyDbContext : DbContext
    {
        private static readonly string connectionString = "Data Source=localhost;Initial Catalog=GalleryDB;Integrated Security=True;";

        // Lazy initialization for the singleton instance
        private static readonly Lazy<MyDbContext> instance = new Lazy<MyDbContext>(() => new MyDbContext());

        // Public constructor to allow instantiation by the factory
        public MyDbContext() : base(connectionString)
        {
        }

        // Public static property to access the singleton instance
        public static MyDbContext SingletonInstance => instance.Value;

        public DbSet<Author> Authors { get; set; }
        public DbSet<WorkOfArt> WorkOfArts { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure entity properties and relationships if needed
            // Example:
            // modelBuilder.Entity<Gallery>().HasKey(g => g.PIB);
        }
    }
}
