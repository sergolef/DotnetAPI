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

            Password ??= "";

            PasswordConfirm ??= "";

            Email ??= "";

            FirstName ??= "";

            LastName ??= "";

            Gender ??= "";

            JobTitle ??= "";

            Department ??= "";
        }

        public string Password { get; set; }
        public string Email { get; set; }
        public string PasswordConfirm { get; set; }
        public string FirstName { get; set; }
        public string LastName { set; get; }
        public string Gender { get; set; }
        public string JobTitle {get;set;}
        public string Department {set; get;}
        public decimal Salary {get;set;}
    }
}