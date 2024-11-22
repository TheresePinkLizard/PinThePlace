using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using PinThePlace.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.NET.StringTools;

namespace PinThePlace.DAL;


public static class DBInit
{
    public static async Task Seed(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        PinDbContext context = serviceScope.ServiceProvider.GetRequiredService<PinDbContext>();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var roles = new List<string> {"Admin","User"};
        foreach (var role in roles)
        {
            if(!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }

        }
        
        if(!context.Users.Any())
        {
            
            var users = new Dictionary<User,string>
             {
                { new User {UserName = "The Student", Email="thestudent@gmail.com"}, "Student123!"},
                { new User {UserName = "HappyDiscoverer", Email="discover@gmail.com"}, "Discover123!"},
                { new User {UserName = "Admin", Email="admin1@gmail.com"}, "Admin123!"},              
            };
            foreach (var u in users)
            {
                var result = await userManager.CreateAsync(u.Key,u.Value);
                if(result.Succeeded)
                {
                    var role = u.Key.UserName == "Admin" ? "Admin" : "User";
                    await userManager.AddToRoleAsync(u.Key,role);
                }
                   
                else
                {
                     throw new Exception(string.Join("\n", result.Errors));
                }
                
            }
        }
        await context.SaveChangesAsync();
        
        //Gets user from database (To avoid proxy or trackingproblems)
        var student = context.Users.FirstOrDefault(u => u.UserName == "The Student");
        var disc = context.Users.FirstOrDefault(u => u.UserName == "HappyDiscoverer");
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
                    UserName = disc.UserName,
                    UserId = disc.Id,
                    ImageUrl = "/images/Map.png",
                     // Users = new List<User> { bruker1 }
                },

                new Pin 
                {
                    Name = "OsloMet",
                    Rating = 5.0m,
                    Comment = "A very good school! I am studying my bachelor!",
                    Latitude = 59.921365321156706, 
                    Longitude = 10.733315263484577,
                    UserName = student.UserName,
                    UserId = student.Id,
                    ImageUrl = "/images/Oslomet.jpg",
                },

                new Pin 
                {
                    Name = "Admin",
                    Rating = 5.0m,
                    Comment = "This is a pin from Admin! Keep sharing your pins!",
                    Latitude = 59.921365321156706, 
                    Longitude = 10.733315263484577,
                    UserName = admin.UserName,
                    UserId = admin.Id,
                    ImageUrl = "/images/sunset.png",
                   // Users = new List<User> { admin }
                }

            };
            context.AddRange(pins);
            context.SaveChanges();
        }
        
    }
}