using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.DTOs
{
    //Information that we want to show to the users.
    public class GenreDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
