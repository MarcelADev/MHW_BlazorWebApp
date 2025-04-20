using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHWApp.Library.Exceptions
{
    public class UnexpectedResponseException : Exception
    {
        private static readonly string MESSAGE_START = "The HTTP Response Statuscode was: ";
        private static readonly string MESSAGE_END= ". This Statuscode is currently not handled";

        public UnexpectedResponseException(string httpStatusCode) : base(MESSAGE_START + httpStatusCode + MESSAGE_END) { }

    }
}
