using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using PinThePlace.Models;

namespace PinThePlace.DAL;


public static class DBInit
{
    public static void Seed(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        PinDbContext context = serviceScope.ServiceProvider.GetRequiredService<PinDbContext>();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        /*
        if(!context.Users.Any())
        {
            var users = new List<User>
            {
                new User
                {
                    UserName = "Bruker1",
                    Email = "bruker1@gmail.com",
                    Password = "123"
                },
                
                new User
                {
                    UserName = "Admin",
                    Email = "admin1@gmail.com",
                    Password = "123"
                }
            };
            context.AddRange(users);
            context.SaveChanges();
        }*/ 
        
        //Gets user from database (To avoid proxy or trackingproblems)
        var bruker1 = context.Users.FirstOrDefault(u => u.UserName == "Bruker1");

        var admin = context.Users.FirstOrDefault(u => u.UserName == "Admin");

        if(!context.Pins.Any())
        {
            var pins = new List<Pin>
            {
                new Pin 
                {
                    Name = "Slottet",
                    Rating = 4.0m,
                    Comment = "Kjempe fin arkitektur og park. Anbefales!",
                    Latitude = 59.91731919136782,
                    Longitude = 10.727738688356991,
                     // Users = new List<User> { bruker1 }
                },

                new Pin 
                {
                    Name = "OsloMet",
                    Rating = 5.0m,
                    Comment = "Bra skole. Anbefales!",
                    Latitude = 59.921365321156706, 
                    Longitude = 10.733315263484577,
                    //Users = new List<User> { bruker1 }
                },

                new Pin 
                {
                    Name = "Admin",
                    Rating = 5.0m,
                    Comment = "Dette er en Admin pin!",
                    Latitude = 59.921365321156706, 
                    Longitude = 10.733315263484577,
                   // Users = new List<User> { admin }
                }

            };
            context.AddRange(pins);
            context.SaveChanges();
        }
        
    }
}