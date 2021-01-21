using MovieApi.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.DTOs
{
    public class MovieDetailsDTO: MoviesDTO
    {
        public List<GenreDTO> Genres { get; set; }
        public List<ActorDTO> Actors { get; set; }
    }
}
