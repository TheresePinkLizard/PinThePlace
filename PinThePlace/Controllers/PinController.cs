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


namespace PinThePlace.Controllers;

public class PinController : Controller
{

    private readonly IPinRepository _pinRepository;
    private readonly UserManager <User> _userManager;
    private readonly ILogger<PinController> _logger;


      public PinController(IPinRepository pinRepository, UserManager<User> userManager, ILogger<PinController> logger)
    {
        _pinRepository = pinRepository;
        _userManager = userManager;
        _logger = logger; 
    }


    //method that returns all pins
    public async Task<IActionResult> Table()
    {  
        var pins = await _pinRepository.GetAll();

        if(!pins.Any())
        {
            _logger.LogError("[PinController] Pin list not found while executing _pinRepository.GetAll()");
            return NotFound("Pin list not found");
        }

        var favorites = await _pinRepository.GetAllFavorites();

        var pinsViewModel = new PinsViewModel(pins,favorites, "Table");

        return View(pinsViewModel);
    }

    
    public async Task<IActionResult> Details(int id)
    {
        var pin = await _pinRepository.GetItemById(id);

        if (pin == null)
            {
            _logger.LogError("[PinController] Pin not found for the PinId {PinId:0000}", id);
            return NotFound("Pin not found for the PinId");
            }
        return View(pin); 
    }

    [HttpGet]
    [Authorize]

    //Create a pin, user has to be logged in to do this
    public IActionResult Create()
    {
        return View();
    }

    
    [HttpPost]
    [Authorize]   
    public async Task<IActionResult> Create(Pin pin)
    {
        if (ModelState.IsValid)
        {
            // Get the current user's ID
            var userName = _userManager.GetUserName(User);
            var userId = _userManager.GetUserId(User);

            if (userName == null)
            {
                return Unauthorized();
            }
            
            // Set the user ID on the pin
            pin.UserName = userName;

            if (userId == null)
            {
                return Unauthorized();
            }
            
            pin.UserId =userId; 

            //taking the uploaded image, and giving it the right path 
            
            var file = pin.UploadedImage;

            if(file != null && file.Length >0) //only if the user has uploaded an image
            {
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/images",fileName);

                using(var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }
                //sets imageurl to be the new filepath of the image
                pin.ImageUrl = "/images/"+fileName;
            }

            bool returnOk= await _pinRepository.Create(pin);
            if (returnOk)
            {
                return RedirectToAction(nameof(Table));
            }
        }
        _logger.LogWarning("[PinController] Pin creation failed {@pin}", pin);
        return View(pin);
    }

    [HttpGet]
    [Authorize]

    //Update pin, user has to be logged in to do this 
    public async Task<IActionResult> Update(int id)  
    {                                   
        // retrieves current user
        var userName = _userManager.GetUserName(User);
        
        // retrieves the pin from the database
        var pin = await _pinRepository.GetItemById(id); 
          
        if (pin == null)               
        {
            _logger.LogError("[PinController] Pin not found when updating the pin {PinId:0000}", id);
            return NotFound("Pin not found for the pinId");

        } else{
             if (userName != "Admin" )
            {
                if(userName != pin.UserName){
                    return Unauthorized();
                }
                
            }
        }
        return View(pin); 
    }

    [HttpPost]
    [Authorize]

    //Takes the information from the form and updates the pin in the database
    public async Task<IActionResult> Update(Pin pin) 
    {   
        
                                               
        if (ModelState.IsValid)
        {
            var file = pin.UploadedImage;
   
            if(file != null && file.Length >0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/images",fileName);
                using(var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }

                pin.ImageUrl = "/images/"+fileName;
            } 

            bool returnOk = await _pinRepository.Update(pin);
            if(returnOk)
            {
            return RedirectToAction(nameof(Table)); // displays the updated list
            }
        }
        _logger.LogWarning("[PinController] Pin update failed {@pin}", pin);
        return View(pin);
    }

    [HttpGet]
    [Authorize]
    //Shows a confirmation page with the pin the user wants to delete 
    public async Task<IActionResult> Delete(int id)  
    {
          // retrieves current user
        var userName = _userManager.GetUserName(User);
        var pin = await _pinRepository.GetItemById(id);  
         
         if (pin == null)               
        {   
            _logger.LogError("[PinController] Pin deleteion failed for {PinId:0000}", id);
            return NotFound("Pin not found for the PinId");

        } else{
            if (userName != "Admin" )
            {
                if(userName != pin.UserName){
                    return Unauthorized();
                }
                
            }
        }
        return View(pin);   
    }

    [HttpPost]
    [Authorize]

    //The method that actually deletes the pin from the database
    public async Task<IActionResult> DeleteConfirmed(int id) 
    {
        bool returnOk = await _pinRepository.Delete(id);  
        if (!returnOk)
        {
            _logger.LogError("[PinController] Pin deletion failed for {PinId:0000}", id);
            return BadRequest("Pin deletion failed");
        }
        return RedirectToAction(nameof(Table)); 
    }
    
}

