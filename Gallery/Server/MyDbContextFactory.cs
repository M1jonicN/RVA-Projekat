using log4net;
using System.Data.Entity.Infrastructure;

namespace Server
{
    public class MyDbContextFactory : IDbContextFactory<MyDbContext>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MyDbContextFactory));

        public MyDbContext Create()
        {
            log.Info("Creating a new instance of MyDbContext.");
            return new MyDbContext();
        }
    }
}
