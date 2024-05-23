﻿using Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Helper;
using Common.DbModels;
using log4net;

namespace Server
{
    public class AuthService : IAuthService
    {
        private static MyDbContext dbContext;
        public AuthService()
        {
            dbContext = MyDbContext.SingletonInstance;
        }

        public User Login(string username, string password)
        {
            if(password == null) return null;
            string passwordHash = HashHelper.ConvertToHash(password);
            User user = dbContext.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == passwordHash);
            if (user !=  null && user.IsLoggedIn == false)
            {
                user.IsLoggedIn = true;
                dbContext.SaveChanges();
                return user;
            }
            return null;
        }


        public bool Register(string username, string password, string firstName, string lastName)
        {
            if (dbContext.Users.Any(u => u.Username == username))
                return false; // Korisničko ime već postoji

            string passwordHash = HashHelper.ConvertToHash(password);
            var newUser = new User {
                Username = username,
                PasswordHash = passwordHash,
                FirstName = firstName,
                UserType = UserType.User,
                LastName = lastName
            };
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

        public User FindUser(string username)
        {
            User user = dbContext.Users.FirstOrDefault(u => u.Username == username);
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
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving user: {ex.Message}");
                return false;
            }
        }

    }
}
