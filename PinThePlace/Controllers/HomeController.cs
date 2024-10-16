using Microsoft.AspNetCore.Mvc;
using PinThePlace.ViewModels;
using PinThePlace.Models; // Assuming your DbContext is in this namespace
using System.Linq;

namespace PinThePlace.Controllers 
{
    public class HomeController : Controller
    {
        private readonly PinDbContext _context;
        public HomeController(PinDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
        var pins = _context.Pins.ToList();
        var viewModel = new PinsViewModel(pins, "Home"); // Using the constructor
        return View(viewModel);
        }
    }
}