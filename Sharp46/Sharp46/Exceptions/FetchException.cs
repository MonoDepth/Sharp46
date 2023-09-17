namespace Sharp46.Exceptions
{
    public class FetchException : Exception
    {
        public FetchException(string message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
