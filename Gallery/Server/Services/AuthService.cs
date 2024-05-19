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

        public User Login(string username, string password)
        {
            if(username == null || password == null) return null;
            string passwordHash = HashHelper.ConvertToHash(password);
            User user = dbContext.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == passwordHash);
            user.IsLoggedIn = true;
            return user;
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
            User user = dbContext.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return false; 
            }

            user.IsLoggedIn = false;
            dbContext.SaveChanges();
            return true;
        }


    }
}
