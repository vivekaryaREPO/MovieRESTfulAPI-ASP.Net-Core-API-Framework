using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieApi.Validations;
using MoviesAPI.DTOs;
using MoviesAPI.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.DTOs
{
    public class MovieCreationDTO:MoviePatchDTO
    {
        [FileSizeValidator(4)]
        [ContentTypeValidator(ContentTypeGroup.image)]
        public IFormFile Poster { get; set; }

        //to receive various genere ids of a movie as a movie can have many genres
        [ModelBinder(BinderType=typeof(TypeBinder<List<int>>))]
        public List<int> GenresIds { get; set; }

        //to receive various characters and their id working in a movie.
        [ModelBinder(BinderType = typeof(TypeBinder<List<ActorDTO>>))]
        public List<ActorDTO> Actors { get; set; }

    }
}
