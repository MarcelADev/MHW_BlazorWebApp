using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHWApp.Library.Exceptions
{
    public class ConflictException : Exception
    {
        public string ConflictTitle { get; set; }

        public ConflictException(string conflictTitle = "")
        {
            ConflictTitle = conflictTitle;
        }
    }
}
