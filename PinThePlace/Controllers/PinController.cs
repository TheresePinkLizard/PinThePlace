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


namespace PinThePlace.Controllers;

public class PinController : Controller
{

    private readonly IPinRepository _pinRepository; // deklarerer en privat kun lesbar felt for å lagre instanser av ItemDbContext

    public PinController(IPinRepository pinRepository) // konstruktør som tar en ItemDbContext instans som et parameter og assigner til _itemDbContext 
    {                                                           // Dette er et eksempel på en dependency injectionm hvor DbContext is provided to the controllerer via ASP.NET Core rammeverk.
        _pinRepository = pinRepository;                         //Konstruktøren blir kalt når en instans er laget, vanligvis under behandling av inkommende HTTP request. Når Views er kalt. eks: table grid, details
    }

    // async i metodene:
    // gjør siden mer responsive. den lar programmet kjøre flere tasks concurrently uten å blokkere main thread.
    // dette får siden til å virke mer responsiv ved å la andre oppgaver gå i forveien istedet for at alt venter på et program.
    // await hører også til async

    // en action som korresponderer til en brukers interaksjon, slik som å liste opp items når en url lastes
    public async Task<IActionResult> Table()
    {  
        // henter alle items fra items table i databasen og konverterer til en liste
        var pins = await _pinRepository.GetAll();

        var pinsViewModel = new PinsViewModel(pins, "Table");
        // en action kan returnere enten: View, JSON, en Redirect, eller annet. 
        // denne returnerer en view
        //Console.WriteLine($"Fetched {pins.Count} pins from the database.");
        return View(pinsViewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        //List<Pin> pins = await _pinDbContext.Pins.ToListAsync();
        //var pin= pins.FirstOrDefault(i => i.PinId == id); // søker igjennom listen items til første som matcher id
        var pin = await _pinRepository.GetItemById(id);
        if (pin == null)
            return NotFound();
        return View(pin); // returnerer view med et item
    }

    //  Http Get og post for å gjøre CRUD
    //Get: It returns a view (the "Create" view) that contains a form where the user can enter details for creating the new item
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Create() // trigges når bruker navigerer til create siden
    {
        return View(); // returnerer view hvor bruker kan skrice inn detaljer for å lage et nytt item
    }

// post:  is used to handle the submission of the form when the user clicks the "Create" button
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(Pin pin) // tar inn item objekt som parameter
    {
        if (ModelState.IsValid) // sjekker validering
        {
            await _pinRepository.Create(pin); // endringer lagres
            return RedirectToAction(nameof(Table)); // redirects to show items in table
        }
        return View(pin);
    }

    // kodene under gjør at update og delete fungerer
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Update(int id)  // denne metoden viser utfyllingsskjemaet for å oppdatere en eksisterende item
    {                                   // metoden slår ut når bruker navigerer seg til update siden
        var pin = await _pinRepository.GetItemById(id); // henter fra database ved hjelp av id
        if (pin == null)               // sjekk om den finner item
        {
            return NotFound();
        }
        return View(pin); 
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Update(Pin pin)  // tar informasjonen som er skrevet i update skjema,
    {                                           // ser hvis det er valid og oppdaterer i database
        if (ModelState.IsValid)
        {
            await _pinRepository.Update(pin);
            return RedirectToAction(nameof(Table)); // displayer den oppdaterte listen
        }
        return View(pin);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Delete(int id)  // displayer confirmation page for å slette en item
    {
        var pin = await _pinRepository.GetItemById(id);  // identifiserer og henter item som skal bli slettet
        if (pin == null)
        {
            return NotFound();
        }
        return View(pin);   // hvis funnet, returnerer view med item data for bekreftelse
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id) // metoden som faktisk sletter item fra database
    {
        await _pinRepository.Delete(id);  // lagrer endringene 
        return RedirectToAction(nameof(Table)); //returnerer bruker til table view hvor item nå er fjernet
    }
    
}

