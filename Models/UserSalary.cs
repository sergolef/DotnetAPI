using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;

namespace DotnetAPI.Models
{
    public partial class UserSalary
    {
        

        public int UserId {get;set;}
        public decimal Salary {get;set;}


        public UserSalary()
        {
            
        }

        
    }
}