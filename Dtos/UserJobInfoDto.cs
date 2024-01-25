using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;

namespace DotnetAPI.Dtos
{
    public partial class UserJobInfoDto
    {
        
        public string JobTitle {get;set;}
        public string Department {set; get;}



        public UserJobInfoDto()
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