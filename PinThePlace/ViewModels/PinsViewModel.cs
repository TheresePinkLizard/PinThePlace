using System.Collections.Generic;
using PinThePlace.Models;

namespace PinThePlace.ViewModels
{
    public class PinsViewModel
    {
        // Egenskaper for alle pins og brukerens pins
        public IEnumerable<Pin> Pins { get; set; }
        public IEnumerable<Pin> UserPins { get; set; }

        // For å holde styr på hvilket view som vises
        public string? CurrentViewName { get; set; }

        // Konstruktør for å kun sette alle pins
        public PinsViewModel(IEnumerable<Pin> pins, string? currentViewName)
        {
            Pins = pins;
            CurrentViewName = currentViewName;
        }

        // Alternativ konstruktør for å sette både pins og brukerens pins
        public PinsViewModel(IEnumerable<Pin> pins, IEnumerable<Pin> userPins, string? currentViewName)
        {
            Pins = pins;
            UserPins = userPins;
            CurrentViewName = currentViewName;
        }
    }
}