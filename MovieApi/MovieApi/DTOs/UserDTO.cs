using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.DTOs
{
    public class UserDTO
    {
        //define the mapping from User to UserDTO
        public string EmailAddress { get; set; }
        public string UserId { get; set; }
    }
}
