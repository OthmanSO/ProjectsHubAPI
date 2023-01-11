using System.Runtime.Serialization;

namespace ProjectsHub.Exceptions
{
    [Serializable]
    internal class PostArssertionFailedException : Exception
    {
        public PostArssertionFailedException()
        {
        }

        public PostArssertionFailedException(string? message) : base(message)
        {
        }

        public PostArssertionFailedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected PostArssertionFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}