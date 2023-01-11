using System.Runtime.Serialization;

namespace ProjectsHub.Exceptions
{
    [Serializable]
    internal class UserDoesNotHavePermissionException : Exception
    {
        public UserDoesNotHavePermissionException()
        {
        }

        public UserDoesNotHavePermissionException(string? message) : base(message)
        {
        }

        public UserDoesNotHavePermissionException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UserDoesNotHavePermissionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}