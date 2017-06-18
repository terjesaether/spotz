using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SpotzWeb.Models;

namespace SpotzWeb.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<SpotzWeb.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "SpotzWeb.Models.ApplicationDbContext";
        }

        protected override void Seed(SpotzWeb.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            const string admin = "Administrator";
            const string user = "User";

            // Create Roles
            if (!context.Roles.Any())
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                var roleAdmin = new IdentityRole { Name = admin };
                var roleUser = new IdentityRole { Name = user };

                roleManager.Create(roleAdmin);
                roleManager.Create(roleUser);
            }

            // Create Users
            if (context.Users.Any()) return;
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            var passwordHash = new PasswordHasher();

            var adminUser = new ApplicationUser
            {
                UserName = "admin@spotz.com",
                Email = "admin@spotz.com",
                PasswordHash = passwordHash.HashPassword("SpotZ123.")
            };

            var user1 = new ApplicationUser
            {
                UserName = "petter@spotz.com",
                Email = "petter@spotz.com",
                PasswordHash = passwordHash.HashPassword("SpotZ456.")
            };

            var user2 = new ApplicationUser
            {
                UserName = "gunnar@spotz.com",
                Email = "gunnar@spotz.com",
                PasswordHash = passwordHash.HashPassword("SpotZ789.")
            };
            var user3 = new ApplicationUser
            {
                UserName = "anja@spotz.com",
                Email = "anja@spotz.com",
                PasswordHash = passwordHash.HashPassword("SpotZ779.")
            };

            userManager.Create(adminUser);
            userManager.Create(user1);
            userManager.Create(user2);
            userManager.Create(user3);
            userManager.AddToRole(adminUser.Id, admin);
            userManager.AddToRole(user1.Id, user);
            userManager.AddToRole(user2.Id, user);
            userManager.AddToRole(user3.Id, user);

            // Spotz
            context.Spotzes.AddOrUpdate(
                s => s.Title,
                new Spotz { SpotzId = Guid.NewGuid(), Title = "Vigelandsparken", Description = "Nice place with lots of naked people", Timestamp = DateTime.Now, User = user1, Latitude = "59.927029", Longitude = "10.6986763" },
                new Spotz { SpotzId = Guid.NewGuid(), Title = "Holmenkollen", Description = "Nice place with lots of ski people", Timestamp = DateTime.Now, User = user2, Latitude = "59.9614673", Longitude = "10.645634" },
                new Spotz { SpotzId = Guid.NewGuid(), Title = "Barcode", Description = "Nice place with lots of tall houses", Timestamp = DateTime.Now, User = adminUser, Latitude = "59.9082535", Longitude = "10.7560058" },
                new Spotz { SpotzId = Guid.NewGuid(), Title = "Tromsø Kino", Description = "Nice place with snow and movies!", Timestamp = DateTime.Now, User = user3, Latitude = "69.6512167", Longitude = "18.9529128" },
                new Spotz { SpotzId = Guid.NewGuid(), Title = "Drammen Kino", Description = "Nice place with lots of tall houses", Timestamp = DateTime.Now, User = adminUser, Latitude = "59.744013", Longitude = "10.200994" },
                new Spotz { SpotzId = Guid.NewGuid(), Title = "Sagene", Description = "Nice place with lots of tall houses", Timestamp = DateTime.Now, User = user2, Latitude = "59.9376758", Longitude = "10.7575062" },
                new Spotz { SpotzId = Guid.NewGuid(), Title = "Tøyen senter", Description = "Nice place with lots of stuff.", Timestamp = DateTime.Now, User = adminUser, Latitude = "59.9148306", Longitude = "10.7651527" },
                new Spotz { SpotzId = Guid.NewGuid(), Title = "Ensjø bilby", Description = "Nice place with lots of cars!", Timestamp = DateTime.Now, User = user1, Latitude = "59.9137359", Longitude = "10.7856459" }
                );
        }
    }
}
