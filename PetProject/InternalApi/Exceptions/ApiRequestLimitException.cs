namespace InternalApi.Exceptions
{
    public class ApiRequestLimitException : Exception
    {
        public ApiRequestLimitException(string? message) : base(message)
        {
        }
    }
}
