using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Validations
{
    public class ContentTypeValidator : ValidationAttribute
    {
        private readonly string[] validContentTypes;
        private readonly string[] imageContentTypes=new string[] { "image/jpeg","image/png","image/gif"};
        public ContentTypeValidator(string[] ValidContentTypes)
        {
            validContentTypes = ValidContentTypes;
        }
        public ContentTypeValidator(ContentTypeGroup contentTypeGroup)
        {
            switch(contentTypeGroup)
            {
                case ContentTypeGroup.image:
                    validContentTypes = imageContentTypes;
                    break;
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            IFormFile formfile = value as IFormFile;
            if (formfile == null)
            {
                return ValidationResult.Success;
            }

            if(!validContentTypes.Contains(formfile.ContentType))
            {
                StringBuilder str = new StringBuilder();
                foreach(string s in validContentTypes)
                {
                    str.Append(s);
                }
                return new ValidationResult("invalid media provided, only media types like: "+str.ToString()+" only");
            }
            return ValidationResult.Success;
        }
    }

    public enum ContentTypeGroup
    {
        image
    }
}
