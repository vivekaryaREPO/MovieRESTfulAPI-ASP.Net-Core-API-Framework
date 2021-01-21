using MovieApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Entities
{
    //A table that keeps movie ids and actors id of actors acted in that movie
    //there can be many actors in one movie=> many to many.
    public class MoviesActors
    {
        public int PersonId { get; set; }
        public int MovieId { get; set; }

        //THIS IS A NAVIGATION PROPERTY, REST IS SCALAR PROPERTIES
        public Person Person { get; set; }

        //THIS IS A NAVIGATION PROPERTY, REST IS SCALAR PROPERTIES
        public Movie Movie { get; set; }
        public string Character { get; set; }
        public int Order { get; set; }
    }
}
