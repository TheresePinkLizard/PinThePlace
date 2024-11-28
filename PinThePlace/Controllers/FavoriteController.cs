using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PinThePlace.Models;
using PinThePlace.ViewModels;
using PinThePlace.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

public class FavoriteController : Controller
{
    private readonly IPinRepository _pinRepository; // deklarerer en privat kun lesbar felt for å lagre instanser av ItemDbContext
    private readonly UserManager <User> _userManager;
    private readonly ILogger<FavoriteController> _logger;

      public FavoriteController(IPinRepository pinRepository, UserManager<User> userManager, ILogger<FavoriteController> logger)
    {
        _pinRepository = pinRepository;
        _userManager = userManager;
        _logger = logger; 
    }

     public async Task<IActionResult> Table()
    {  
        // henter alle items fra items table i databasen og konverterer til en liste
        var favorites = await _pinRepository.GetAllFavorites();

        var pins = await _pinRepository.GetAll();
        if(!pins.Any())
        {
            _logger.LogError("[PinController] Pin list not found while executing _pinRepository.GetAll()");
            return NotFound("Pin list not found");
        }

        if(!favorites.Any())
        {
            _logger.LogError("[PinController] Favorite list not found while executing _pinRepository.GetAllFavorties()");
            return NotFound("Favorite list not found");
        }

        var pinsViewModel = new PinsViewModel(pins,favorites, "Table");
        // en action kan returnere enten: View, JSON, en Redirect, eller annet. 
        // denne returnerer en view
        //Console.WriteLine($"Fetched {pins.Count} pins from the database.");
        return View(pinsViewModel);
    }

    [HttpGet]
    [Authorize]
    public IActionResult AddToFavorites(int id)
    {
        var favorite = new Favorite
        {
            PinId = id,
            UserId = _userManager.GetUserId(User)
        };
    return View("CreateFavorite",favorite);
    }

    [HttpPost]
    [Authorize]// Ensure the user is logged in
    public async Task<IActionResult> AddToFavorites(Favorite favorite)
    {

    // Fetch the Pin with the given ID
        favorite.Pin = await _pinRepository.GetItemById(favorite.PinId);
        favorite.User = await _userManager.FindByIdAsync(favorite.UserId);
        
        var success = await _pinRepository.SaveFavorite(favorite);

        if (success)
        {
             return RedirectToAction(nameof(Table),"Pin");
        }
         _logger.LogWarning("[FavoriteController] Favorite creation failed {@favorite}", favorite);
        return View(favorite);


    }



 



}