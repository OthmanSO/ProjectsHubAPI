using System.Runtime.Serialization;

namespace ProjectsHub.Exceptions
{
    [Serializable]
    internal class UserPasswordNotMatchedException : Exception
    {
        public UserPasswordNotMatchedException()
        {
        }

        public UserPasswordNotMatchedException(string? message) : base(message)
        {
        }

        public UserPasswordNotMatchedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UserPasswordNotMatchedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}