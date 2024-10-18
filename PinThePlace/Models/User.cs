namespace PinThePlace.Models;

public class User
{
    public int UserId{ get; set; }

    public string UserName{get; set;} = string.Empty;

    //public string FirstName{ get; set; } = string.Empty;

    //public string LastName{ get; set; } = string.Empty;

    public string Email{ get; set; } = string.Empty;

    public virtual List<Pin>? Pins {get;set;}

    public string Password{ get; set; } = string.Empty;

}