using Microsoft.AspNetCore.Mvc;
using PinThePlace.Models;
using Microsoft.EntityFrameworkCore;

namespace PinThePlace.Controllers;

public class UserController : Controller
{
    private readonly UserDbContext _userDbContext;

    public UserController(UserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
    }

    public async Task<IActionResult> Table()
    {
        List<User> users = await _userDbContext.Users.ToListAsync();
        return View(users);
    }

}