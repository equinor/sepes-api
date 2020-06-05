using System;

namespace Sepes.Infrastructure.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
        public NotFoundException() : base("Not found") { }
        public NotFoundException(string message, System.Exception inner) : base(message, inner) { }
        protected NotFoundException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public static NotFoundException CreateForIdentity(string entityName, int id, Exception inner = null)
        {
            return new NotFoundException($"{entityName} with id {id} not found!", inner);
        }
    }
}
