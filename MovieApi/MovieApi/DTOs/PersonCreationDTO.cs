using Microsoft.AspNetCore.Http;
using MovieApi.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.DTOs
{
    //sometimes inheritance is not suggestable as understanding may get clumsy
    public class PersonCreationDTO:PersonPatchDTO
    {

        [FileSizeValidator(4)]
        [ContentTypeValidator(ContentTypeGroup.image)]
        public IFormFile Picture { get; set; }
    }
}
