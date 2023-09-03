namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Exceptions
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
