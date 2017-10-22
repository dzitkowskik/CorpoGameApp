using System;

namespace CorpoGameApp.Services
{
    internal class PlayerAlreadyQeueuedException : Exception
    {
        public PlayerAlreadyQeueuedException()
        {
        }

        public PlayerAlreadyQeueuedException(string message) : base(message)
        {
        }

        public PlayerAlreadyQeueuedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}