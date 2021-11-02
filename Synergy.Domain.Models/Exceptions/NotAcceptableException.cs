using System;

namespace Synergy.Domain.Models.Exceptions
{
    public class NotAcceptableException : ApplicationException
    {
        public NotAcceptableException()
        {
        }

        public NotAcceptableException(string message) : base(message)
        {
        }

        public NotAcceptableException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
