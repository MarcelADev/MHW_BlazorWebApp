using MHWApp.Library.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHWApp.Library.DTO
{
    public class CreateSessionDTO
    {
        public required Session NewSession { get; set; }
        public required List<Participator> Participators { get; set; }
    }
}
