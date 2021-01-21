using MovieApi.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.DTOs
{
    public class IndexMoviePageDTO
    {
        public List<MoviesDTO> UpcomingReleases { get; set; }
        public List<MoviesDTO> InTheaters { get; set; }
    }
}
