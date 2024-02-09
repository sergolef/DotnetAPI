using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetAPI.Dtos
{
    public class PostToEditDto
    {
        
        public PostToEditDto()
        {
            PostTitle ??="";
            PostContent ??="";
        }

        public int PostId { get; set; }
        public string PostTitle { get; set; }

        public string PostContent { get; set; }
    }
}