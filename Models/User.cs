using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; //for [NotMapped]
namespace FavoriteMovies.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage="Please Provide First Name")]
        [MinLength(2, ErrorMessage="First name must be more than 2 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage="Please Provide Last Name")]
        [MinLength(2, ErrorMessage="Last name must be more than 2 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage="Please Provide Email")]
        [EmailAddress(ErrorMessage="Provide Valid Email")]
        public string Email { get; set; }
        public string Password { get; set; }

        [NotMapped]
        [Compare("Password")]
        // [DataType(DataType.Password)]
        public string Confirm {get;set;}

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public List<Movie> PostedMovies {get;set;}
        public List<Like> Likes {get;set;}
    }
}