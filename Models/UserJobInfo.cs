using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;

namespace DotnetAPI.Models
{
    public partial class UserJobInfo
    {
        

        public int UserId {get;set;}
        public string JobTitle {get;set;}
        public string Department {set; get;}



        public UserJobInfo()
        {
           if(JobTitle == null)
           {
                JobTitle = "";
           }

           if(Department == null)
            {
                Department = "";
            }

        }

        
    }
}