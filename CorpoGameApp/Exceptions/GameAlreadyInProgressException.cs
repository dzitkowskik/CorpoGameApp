using System;

namespace CorpoGameApp.Services
{
    internal class GameAlreadyInProgressException : Exception
    {
        public GameAlreadyInProgressException()
        {
        }

        public GameAlreadyInProgressException(string message) : base(message)
        {
        }

        public GameAlreadyInProgressException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}