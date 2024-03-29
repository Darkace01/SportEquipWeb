﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SportEquipWeb.Data;
using SportEquipWeb.Models.Core;

namespace SportEquipWeb.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            
    
            return userIdentity;
        }
        public IEnumerable<Transaction> Transactions { get; set; }
        public bool IsEnabled { get; set; }

    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new DbInitializer());
        }

        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        //public System.Data.Entity.DbSet<SportEquipWeb.Models.ApplicationUser> ApplicationUsers { get; set; }

        //public System.Data.Entity.DbSet<SportEquipWeb.Models.ApplicationUser> ApplicationUsers { get; set; }

        //public System.Data.Entity.DbSet<SportEquipWeb.Models.ApplicationUser> ApplicationUsers { get; set; }
    }
}