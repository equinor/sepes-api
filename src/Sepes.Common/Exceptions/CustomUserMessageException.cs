using System;
using System.Net;

namespace Sepes.Common.Exceptions
{
    public class CustomUserMessageException : Exception
    {
        public string UserFriendlyMessage { get; private set; }

        public HttpStatusCode? StatusCode { get; private set; }

        public CustomUserMessageException(string message, string userFriendlyMessage = null, HttpStatusCode? httpStatusCode = default) : base(message)
        {
            UserFriendlyMessage = userFriendlyMessage;
            StatusCode = httpStatusCode;
        }
        
        public CustomUserMessageException(string message, Exception inner, string userFriendlyMessage = null, HttpStatusCode? httpStatusCode = default) : base(message, inner)  {
            UserFriendlyMessage = userFriendlyMessage;
            StatusCode = httpStatusCode;
        }
        
        protected CustomUserMessageException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}