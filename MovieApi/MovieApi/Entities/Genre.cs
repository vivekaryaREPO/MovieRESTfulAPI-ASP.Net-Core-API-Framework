using MoviesAPI.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.Entities
{
    public class Genre
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        //get all movies of a specific genre. It's obvious as Genre is a foreign key in MoviesGenres table.
        public List<MoviesGenres> MoviesGenres { get; set; }
    }
}
