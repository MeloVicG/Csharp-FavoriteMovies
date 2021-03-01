using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace FavoriteMovies.Models
{
    public class Movie
    {
        [Key]
        public int MovieId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Starring { get; set; }
        [Required]
        public string ImageURL { get; set; }
        [Required]
        
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        //how do we get the poster
        
        public int UserId{get;set;} //this is our foreign key
        public User PostedBy{get;set;}
        //how do we get the fans
        public List<Like> Likes {get;set;}
    }
}