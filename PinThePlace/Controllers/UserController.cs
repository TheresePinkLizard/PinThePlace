using Microsoft.AspNetCore.Mvc;
using PinThePlace.Models;
using Microsoft.EntityFrameworkCore;

namespace PinThePlace.Controllers;

public class UserController : Controller
{
    private readonly PinDbContext _pinDbContext;

    public UserController(PinDbContext pinDbContext)
    {
        _pinDbContext = pinDbContext;
    }

    public async Task<IActionResult> Table()
    {
        List<User> users = await _pinDbContext.Users.ToListAsync();
        return View(users);
    }

}