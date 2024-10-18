using Microsoft.EntityFrameworkCore;
namespace PinThePlace.Models;

public static class DBInit
{
    public static void Seed(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        PinDbContext context = serviceScope.ServiceProvider.GetRequiredService<PinDbContext>();
        //context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        if(!context.Pins.Any())
        {
            var pins = new List<Pin>
            {
                // Legg inn Pins
            };
            context.AddRange(pins);
            context.SaveChanges();
        }

        if(!context.Users.Any())
        {
            var users = new List<User>
            {
                // Legg inn Users
            };
            context.AddRange(users);
            context.SaveChanges();
        }

        
    }
}