using Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Helper;
using Common.DbModels;

namespace Server
{
    public class AuthService : IAuthService
    {
        private readonly MyDbContext dbContext;

        public AuthService()
        {
            dbContext = new MyDbContext();
        }

        public bool Login(string username, string password)
        {
            string passwordHash = HashHelper.ConvertToHash(password);
            var user = dbContext.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == passwordHash);
            return user != null;
        }

        public bool Register(string username, string password)
        {
            if (dbContext.Users.Any(u => u.Username == username))
                return false; // Korisničko ime već postoji

            string passwordHash = HashHelper.ConvertToHash(password);
            var newUser = new User { Username = username, PasswordHash = passwordHash };
            dbContext.Users.Add(newUser);
            dbContext.SaveChanges();
            return true;
        }

        public bool Logout(string username)
        {
            return true; 
        }
    }
}
