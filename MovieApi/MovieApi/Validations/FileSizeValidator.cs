using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.Validations
{
    public class FileSizeValidator : ValidationAttribute
    {
        private readonly int maxFileLength;
        public FileSizeValidator(int fileLength)
        {
            maxFileLength = fileLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value==null)
            {
                return ValidationResult.Success;
            }
            IFormFile formfile = value as IFormFile;
            if(formfile==null)
            {
                return ValidationResult.Success;
            }
            if(formfile.Length>maxFileLength*1024*1024)
            {
                return new ValidationResult("File size cant be larger than "+ maxFileLength+" size");
            }
            return ValidationResult.Success;
        }
    }
}
