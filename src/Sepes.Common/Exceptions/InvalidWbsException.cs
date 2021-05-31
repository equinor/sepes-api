using System;

namespace Sepes.Common.Exceptions
{
    public class InvalidWbsException : CustomUserMessageException
    {
        public InvalidWbsException(string message, string userFriendlyMessage, Exception innerException = null) : base(message, innerException, userFriendlyMessage, System.Net.HttpStatusCode.BadRequest) { }
    }
}
