using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetAPI.Dtos
{
    public class PostToAddDto
    {
        
        public PostToAddDto()
        {
            PostTitle ??="";
            PostContent ??="";
        }

        public string PostTitle { get; set; }

        public string PostContent { get; set; }

    }
}