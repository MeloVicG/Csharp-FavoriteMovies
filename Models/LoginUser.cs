using System;
using System.ComponentModel.DataAnnotations;

namespace FavoriteMovies.Models
{
public class LoginUser
    {   
        [Required(ErrorMessage="Please Provide Email")]
        [EmailAddress(ErrorMessage="Provide Valid Email")]
        public string LoginEmail { get; set; }

        public string LoginPassword {get;set;}
        
    }
}