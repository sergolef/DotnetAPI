using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;

namespace DotnetAPI.Models
{
    public partial class User
    {
        

        public int UserId {get;set;}
        public string FirstName {get;set;}
        public string LastName {set; get;}
        public string Email {get; set;}
        public string Gender {get; set;}
        public bool Active {get; set;}


        public User()
        {
            FirstName ??= "";
            LastName ??= ""; 
            Email ??= "";
            Gender ??= "";
        }

        
    }
}