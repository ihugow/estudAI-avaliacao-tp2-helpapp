using System;
using System.Runtime.Serialization;

namespace StockApp.Application.Exceptions
{
    [Serializable]
    public class InvalidCredentialsException : AuthenticationException
    {
        public InvalidCredentialsException() : base() { } // Construtor padrão

        public InvalidCredentialsException(string message = "Invalid credentials provided.") : base(message) { }

        public InvalidCredentialsException(string message, Exception innerException) : base(message, innerException) { }

        protected InvalidCredentialsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}