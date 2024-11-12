using System;
namespace PinThePlace.Models
{
    public class Pin {
        public int PinId { get; set; }
        public string Name { get; set; } = string.Empty; //  = string.Empty;  is to state this is a mandatory value, has empthy string by default. 
                                        //can also use ? on nullable variabels instead, like this code shows
        public decimal Rating { get; set; }         
        public string? Comment { get; set; }    
        public string? ImageUrl { get; set; }
        
        // adds date for when object is created
        public DateTime DateCreated { get; set; } = DateTime.Now;
        //navigation property
        public virtual List<User>? Users { get; set; }

    }
}