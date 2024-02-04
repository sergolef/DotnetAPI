using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetAPI.Dtos
{
    public partial class UserForLoginConfirmationDto
    {
        public UserForLoginConfirmationDto()
        {
            if(PasswordHash == null)
            {
                PasswordHash = new byte[0];
            }

            if(PasswordSolt == null)
            {
                PasswordSolt = new byte[0];
            }
        }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSolt { get; set; }

    }
}