using Server.DbModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class MyDbContext : DbContext
    {
        private static string connectionString = "Data Source=localhost;Initial Catalog=GalleryDB;Integrated Security=True;";

        public MyDbContext() : base(connectionString)
        {
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<WorkOfArt> WorkOfArts { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
