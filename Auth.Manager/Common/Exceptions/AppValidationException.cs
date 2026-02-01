namespace Auth.Manager.Common.Exceptions
{
    public sealed class AppValidationException : Exception
    {
        public AppValidationException(string message)
            : base(message)
        {
        }
    }
}
