using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;

namespace DotnetAPI.Models
{
    public partial class UserComplete
    {
        
        public int UserId {get;set;}
        public string FirstName {get;set;}
        public string LastName {set; get;}
        public string Email {get; set;}
        public string Gender {get; set;}
        public bool Active {get; set;}

        public string JobTitle {get;set;}
        public string Department {set; get;}
        public decimal Salary {get;set;}

        public UserComplete()
        {
            FirstName ??= "";
            LastName ??= ""; 
            Email ??= "";
            Gender ??= "";
            JobTitle ??= "";
            Department ??= "";
        }

        
    }
}