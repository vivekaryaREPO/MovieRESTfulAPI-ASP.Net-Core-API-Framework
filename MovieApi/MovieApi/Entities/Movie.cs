using MoviesAPI.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.Entities
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        [StringLength(300)]
        public string Title { get; set; }
        public string Summary { get; set; }
        public bool InTheaters { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string  Poster { get; set; }


        //A movie has actors, many actors. Hence we might want to get all the actors corresponding to a movie
        //a movie, so when you put this List here, It's obvious that we want to have a property
        //i.e a column Movie as secondary key in MoviesActors table. That's how entity framework core 
        //will connect them i.e MoviesGenres and Movie.
        //So MoviesActors is our many-to-many relationship
        //THIS IS A NAVIGATION PROPERTY, REST IS SCALAR PROPERTIES
        //So Movie.MoviesActors we are trying to get list of other objects
        public List<MoviesActors> MoviesActors { get; set; }

        //A movie has genre, many genres. Hence we might want to get all the genres corresponding to a genre
        //a movie, so when you put this List here, It's obvious that we want to have a property
        //i.e a column Movie as secondary key in MoviesGenres table. That's how entity framework core 
        //will connect these 2 tables i.e MoviesGenres and Movie.
        //So MoviesGenres is our many-to-many relationship
        //THIS IS A NAVIGATION PROPERTY, REST IS SCALAR PROPERTIES
        //So Movie.MoviesGenres means we are trying to get list of other objects

        public List<MoviesGenres> MoviesGenres { get; set; }

    }
}
