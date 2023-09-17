using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharp46.Exceptions
{
    public class FetchException: Exception
    {
        public FetchException(string message, Exception? innerException): base(message, innerException)
        {
        }
    }
}
