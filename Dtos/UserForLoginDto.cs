using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetAPI.Dtos
{
    public class UserForLoginDto
    {
        public UserForLoginDto()
        {
            if (Password == null)
            {
                Password = "";
            }

            if(Email == null)
            {
                Email = "";
            }
        }

        public string Password { get; set; }
        public string Email { get; set; }  
    
    }
}