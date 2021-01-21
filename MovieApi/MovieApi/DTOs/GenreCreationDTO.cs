using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.DTOs
{
    //We had to create this because the GenreDTO has 2 parameters and our post() in controller
    //needs only one parameter set i.e Name.
    public class GenreCreationDTO
    {
        [Required]
        public string Name { get; set; }
    }
}
