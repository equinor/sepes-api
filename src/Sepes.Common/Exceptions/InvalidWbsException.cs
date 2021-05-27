using System;

namespace Sepes.Common.Exceptions
{
    public class InvalidWbsException : CustomUserMessageException
    {
        public InvalidWbsException(string message, string userFriendlyMessage) : base(message, userFriendlyMessage) { }
    }
}
