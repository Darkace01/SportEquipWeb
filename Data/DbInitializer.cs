using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SportEquipWeb.Models;
using SportEquipWeb.Models.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SportEquipWeb.Data
{
    public class DbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        public DbInitializer()
        {

        }

        protected override void Seed(ApplicationDbContext context)
        {
            

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            //check if admin exists  
            if (!roleManager.RoleExists("Admin"))
            {

                //create admin role
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                //create admin             

                var user = new ApplicationUser();
                user.UserName = "admin@gmail.com";
                user.Email = "admin@gmail.com";
                user.IsEnabled = true;

                string userPWD = "abc123";

                var chkUser = UserManager.Create(user, userPWD);

                //add adminRole to admin
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "admin");

                }
            }


            //check if manager exists  
            if (!roleManager.RoleExists("Owner"))
            {

                //create manager role
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Owner";
                roleManager.Create(role);

                //create Result manager             

                var user = new ApplicationUser();
                user.UserName = "owner@gmail.com";
                user.Email = "owner@gmail.com";

                user.IsEnabled = true;


                string userPWD = "abc123";

                var chkUser = UserManager.Create(user, userPWD);

                //add managerRole to manager  
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "owner");

                }
            }
            if (!roleManager.RoleExists("User"))
            {

                //create manager role
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "User";
                roleManager.Create(role);

                //create Result manager             

                var user = new ApplicationUser();
                user.UserName = "user@gmail.com";
                user.Email = "user@gmail.com";

                user.IsEnabled = true;

                string userPWD = "abc123";

                var chkUser = UserManager.Create(user, userPWD);

                //add managerRole to manager  
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "user");

                }
            }
            var category = new List<Category>
            {
                new Category{Name="Field"},
                new Category{Name="Track"}
            };
            category.ForEach(s => context.Category.Add(s));
            context.SaveChanges();
            base.Seed(context);
        }
    }
}