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
    private readonly IPinRepository _pinRepository; 
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
    
        //Gathers all favorites in the favorites table in the database 
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
    
        return View(pinsViewModel);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> AddToFavorites(int id)
    {   
        var user = _userManager.GetUserId(User);
        var pin = await _pinRepository.GetItemById(id);

        if(user == null)
        {
            _logger.LogError("FavoriteController] User not found when trying to favorite pin {PinId:0000}", id);
            return NotFound("User not found, can't create favorite");
        }

        if(pin == null)
        {
            _logger.LogError("FavoriteController] Pin not found when trying to favorite pin {PinId:0000}", id);
            return NotFound("Pin not found for the pinId, cant create favorite");
        }

        var favorite = new Favorite
        {
            PinId = pin.PinId,
            UserId=user,
            MadeBy=pin.UserName,
        };
       
    return View("CreateFavorite",favorite);
    }

    [HttpPost]
    [Authorize]// Ensure the user is logged in
    public async Task<IActionResult> AddToFavorites(Favorite favorite)
    {
        if(ModelState.IsValid)
        {
           favorite.Pin = await _pinRepository.GetItemById(favorite.PinId);
           favorite.User = await _userManager.FindByIdAsync(favorite.UserId);
        }

        bool returnOk = await _pinRepository.SaveFavorite(favorite);

        if (returnOk)
        {
             return RedirectToAction(nameof(Table),"Pin");
        }
         _logger.LogWarning("[FavoriteController] Favorite creation failed {@favorite}", favorite);
        return View("CreateFavorite",favorite);

    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> UpdateFavorite(int id) 
    {                                   
        //gets the logged in user
        var userName = _userManager.GetUserName(User);
        var userId = _userManager.GetUserId(User);
       
        
        // gets the favorite from the database based on the Id
        var favorite = await _pinRepository.GetFavoriteById(id); 
        if(favorite == null)
        {
            _logger.LogError("[FavoriteController] Favorite not found when updating the Favorite {FavoriteId:0000}", id);
            return NotFound("Favorite not found for the FavoriteId");
        }
        var pin = await _pinRepository.GetItemById(favorite.PinId);

        if(pin == null)
        {
            _logger.LogError("[FavoriteController] Corresponding pin not found when updating the Favorite {FavoriteId:0000}", id);
            return NotFound("Favorite not found for the FavoriteId");
        }
          
        if (favorite == null)              
        {
            _logger.LogError("[FavoriteController] Favorite not found when updating the Favorite {FavoriteId:0000}", id);
            return NotFound("Favorite not found for the FavoriteId");

        } else{
             if (userName != "Admin" )
            {
                if(userId != favorite.UserId){
                    return Unauthorized();
                }
                
            }
        
        }

        favorite.MadeBy=pin.UserName;
        return View(favorite); 
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UpdateFavorite(Favorite favorite)  
    {   
        
        if (ModelState.IsValid)
        {
            bool returnOk = await _pinRepository.UpdateFavorite(favorite);
            if(returnOk)
            {
            return RedirectToAction(nameof(Table),"Pin"); 
            }
        }
        _logger.LogWarning("[FavoriteController] Favorite update failed {@favorite}", favorite);
        return RedirectToAction(nameof(Table),"Pin");
    }


    [HttpGet]
    [Authorize]
    public async Task<IActionResult> DeleteFavorite(int id)  
    {
          // get the current logged in user
        var userName = _userManager.GetUserName(User);
        var userId =  _userManager.GetUserId(User);

        //Gets the favorite that is going to be deleted based on the Id
        var favorite = await _pinRepository.GetFavoriteById(id); 
         
         if (favorite == null)               
        {   
            _logger.LogError("[FavoriteController] Favorite deletion failed for {FavortieId:0000}", id);
            return NotFound("Favorite not found for the FavoriteId");

        } 
       return View("DeleteFavorite",favorite);   
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id) 
    {
        bool returnOk = await _pinRepository.DeleteFavorite(id);   
        if (!returnOk)
        {
            _logger.LogError("[FavoriteController] Favorite deletion failed for {FavoriteId:0000}", id);
            return BadRequest("Favorite deletion failed");
        }
        return RedirectToAction(nameof(Table),"Pin"); 
    }




 



}