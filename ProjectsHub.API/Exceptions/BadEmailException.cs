using System.Runtime.Serialization;

namespace ProjectsHub.Exceptions
{
    [Serializable]
    internal class BadEmailException : Exception
    {
        public BadEmailException()
        {
        }

        public BadEmailException(string? message) : base(message)
        {
        }

        public BadEmailException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected BadEmailException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}