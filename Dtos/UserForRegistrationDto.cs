using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetAPI.Dtos
{
    public class UserForRegistrationDto
    {
        public UserForRegistrationDto()
        {
            if (Password == null)
            {
                Password = "";
            }

            if(PasswordConfirm == null)
            {
                PasswordConfirm = "";
            }

            if(Email == null)
            {
                Email = "";
            }
        }

        public string Password { get; set; }
        public string Email { get; set; }  
        public string PasswordConfirm { get; set; } 
    }
}