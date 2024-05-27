using Common.Contracts;
using Common.Helper;
using Common.DbModels;
using log4net;
using System.Linq;
using System;

namespace Server
{
    public class AuthService : IAuthService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AuthService));
        private static MyDbContext dbContext;

        public AuthService()
        {
            dbContext = MyDbContext.SingletonInstance;
            log.Info("AuthService instance created.");
        }

        #region Methods
        public User Login(string username, string password)
        {
            if (password == null) return null;
            string passwordHash = HashHelper.ConvertToHash(password);
            User user = dbContext.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == passwordHash);
            if (user != null && user.IsLoggedIn == false)
            {
                user.IsLoggedIn = true;
                dbContext.SaveChanges();
                log.Info($"User {username} logged in.");
                return user;
            }
            log.Warn($"Login failed for user {username}.");
            return null;
        }

        public bool Register(string username, string password, string firstName, string lastName)
        {
            if (dbContext.Users.Any(u => u.Username == username))
            {
                log.Warn($"Registration failed: Username {username} already exists.");
                return false; // Korisničko ime već postoji
            }

            string passwordHash = HashHelper.ConvertToHash(password);
            var newUser = new User
            {
                Username = username,
                PasswordHash = passwordHash,
                FirstName = firstName,
                UserType = UserType.User,
                LastName = lastName
            };
            dbContext.Users.Add(newUser);
            dbContext.SaveChanges();
            log.Info($"User {username} registered successfully.");
            return true;
        }

        public bool Logout(string username)
        {
            User user = dbContext.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                log.Warn($"Logout failed: User {username} not found.");
                return false;
            }

            user.IsLoggedIn = false;
            dbContext.SaveChanges();
            log.Info($"User {username} logged out.");
            return true;
        }

        public User FindUser(string username)
        {
            User user = dbContext.Users.FirstOrDefault(u => u.Username == username);
            log.Info($"User {username} found.");
            return user;
        }

        public bool SaveChanges(User user)
        {
            try
            {
                var existingUser = dbContext.Users.FirstOrDefault(u => u.ID == user.ID);
                if (existingUser != null)
                {
                    existingUser.FirstName = user.FirstName;
                    existingUser.LastName = user.LastName;
                    existingUser.Username = user.Username;
                    existingUser.PasswordHash = user.PasswordHash;

                    dbContext.SaveChanges();
                    log.Info($"User {user.Username} updated successfully.");
                    return true;
                }

                log.Warn($"SaveChanges failed: User {user.Username} not found.");
                return false;
            }
            catch (Exception ex)
            {
                log.Error($"Error saving user: {ex.Message}", ex);
                return false;
            }
        }
        #endregion
    }
}
