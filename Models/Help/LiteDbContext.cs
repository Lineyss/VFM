﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using VFM.Models.Users;

namespace VFM.Models.Help
{
    public class LiteDbContext : DbContext
    {
        public LiteDbContext(DbContextOptions<LiteDbContext> options) : base(options)
        {
            Database.EnsureCreated();
            createSuperUser();
        }

        public void createSuperUser()
        {
            var _user = user.FirstOrDefault(user => user.login == "admin");

            if (_user == null)
            {
                user.Add(new User
                {
                    login = "admin",
                    password = HashPassword.Hash("admin"),
                    isAdmin = true,
                    createF = true,
                    deleteF = true,
                    updateNameF = true,
                    downloadF = true,
                    uploadF = true
                });

                SaveChanges();
            }
        }

        public DbSet<User> user { get; set; }
    }
}