using Common.DbModels;
using log4net;
using System;
using System.Data.Entity;

namespace Server
{
    public class MyDbContext : DbContext
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MyDbContext));
        private static readonly string connectionString = "Data Source=localhost;Initial Catalog=GalleryDB;Integrated Security=True;";

        // Lazy initialization for the singleton instance
        private static readonly Lazy<MyDbContext> instance = new Lazy<MyDbContext>(() => new MyDbContext());

        // Public constructor to allow instantiation by the factory
        public MyDbContext() : base(connectionString)
        {
            log.Info("MyDbContext instance created.");
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
            log.Info("OnModelCreating called.");
            // Configure entity properties and relationships if needed
            // Example:
            // modelBuilder.Entity<Gallery>().HasKey(g => g.PIB);
        }
    }
}
