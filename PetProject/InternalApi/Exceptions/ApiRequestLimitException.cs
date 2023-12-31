﻿namespace InternalApi.Exceptions
{
    /// <summary>
    /// Исключение о превышении лимита запросов в внешнему Api
    /// </summary>
    public class ApiRequestLimitException : Exception
    {
        public ApiRequestLimitException(string? message) : base(message)
        {
        }
    }
}
