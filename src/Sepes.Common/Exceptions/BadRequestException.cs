using System;

namespace Sepes.Common.Exceptions
{
    public class BadRequestException : CustomUserMessageException
    {
        public BadRequestException(string message) : base(message) { }

        public BadRequestException(string message, string userFriendlyMessage, Exception innerException = null) : base(message, innerException, userFriendlyMessage, System.Net.HttpStatusCode.BadRequest) { }

        public BadRequestException(string message, System.Exception inner) : base(message, inner) { }
        protected BadRequestException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
