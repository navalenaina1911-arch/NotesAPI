namespace Notes.Exceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; }
        public object? Details { get; }

        public ApiException(int statusCode, string message, object? details = null)
            : base(message)
        {
            StatusCode = statusCode;
            Details = details;
        }
    }
}