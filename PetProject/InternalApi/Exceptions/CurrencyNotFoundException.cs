namespace InternalApi.Exceptions
{
    /// <summary>
    /// Исключени о ненайденной валюте
    /// </summary>
    public class CurrencyNotFoundException : Exception
    {
        public CurrencyNotFoundException(string? message) : base(message)
        {
        }
    }
}
