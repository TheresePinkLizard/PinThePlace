using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinThePlace.Models
{
    public class Favorite {

        public int FavoriteId {get; set;}

        public string? Category {get; set;}

        public int PinId { get; set; }

        public string UserName {get; set;} = string.Empty;


        public virtual User? User { get; set; }

        public virtual Pin? Pin { get; set; }



    }
}