using MHWApp.Library.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHWApp.Library.DTO
{
    public class SessionUpdateDTO
    {
        public required Session UpdatedSession { get; set; }
        public required List<Participator> RemoveParticipators { get; set; }
        public required List<Participator> AddParticipators { get; set; }
    }
}
