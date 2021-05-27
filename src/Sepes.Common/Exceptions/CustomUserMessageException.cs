using System;

namespace Sepes.Common.Exceptions
{
    public class CustomUserMessageException : Exception
    {
        public string UserFriendlyMessage { get; private set; }

        public CustomUserMessageException(string message, string userFriendlyMessage = null) : base(message)
        {
            UserFriendlyMessage = userFriendlyMessage; 
        }
        
        public CustomUserMessageException(string message, System.Exception inner, string userFriendlyMessage = null) : base(message, inner)  {
            UserFriendlyMessage = userFriendlyMessage; 
        }
        
        protected CustomUserMessageException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}