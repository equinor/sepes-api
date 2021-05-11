using System;

namespace Sepes.Infrastructure.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message) { }
        public ForbiddenException() : base("Forbidden") { }
        public ForbiddenException(string message, System.Exception inner) : base(message, inner) { }
        protected ForbiddenException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
