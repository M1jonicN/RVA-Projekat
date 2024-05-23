using System.Data.Entity.Infrastructure;

namespace Server
{
    public class MyDbContextFactory : IDbContextFactory<MyDbContext>
    {
        public MyDbContext Create()
        {
            return new MyDbContext();
        }
    }
}
