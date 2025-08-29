using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHWApp.Library.DTO.Identity
{
    public class IdentityDTO
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}
