using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharp46.Exceptions
{
    /// <summary>
    /// Thrown during an error when attempting to send a message
    /// See the innerException for more details
    /// </summary>
    public class SendMessageException: Exception
    {
        /// <summary>
        /// Thrown during an error when attempting to send a message
        /// </summary>
        /// <param name="message">The reason for the exception</param>
        /// <param name="innerException">The underlying exception that trigged this exception to be raised</param>
        public SendMessageException(string message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
