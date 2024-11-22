using Microsoft.AspNetCore.Mvc;
using PinThePlace.Models;
using Microsoft.EntityFrameworkCore;
using PinThePlace.DAL;
using Microsoft.AspNetCore.Identity;

namespace PinThePlace.Controllers;

public class UserController : Controller
{
    private readonly PinDbContext _pinDbContext;
     private readonly UserManager<User> _userManager;

   public UserController(PinDbContext pinDbContext, UserManager<User> userManager)
        {
            _pinDbContext = pinDbContext;
            _userManager = userManager;
        }

    public async Task<IActionResult> Table()
    {
        List<User> users = await _pinDbContext.Users.ToListAsync();

        var userName = _userManager.GetUserName(User);
        
        if (userName != "Admin" )
        {
            return Unauthorized();
            
        }else{
            return View(users);
        }
        
    }
     public async Task<IActionResult> MyPins()
        {
            // Get the current user's ID
            var userId = _userManager.GetUserId(User);

            // Get the user and their pins from the database
            var user = await _pinDbContext.Users
                .Include(u => u.Pins)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            // Pass the pins to the view
            return View(user.Pins);
        }
}