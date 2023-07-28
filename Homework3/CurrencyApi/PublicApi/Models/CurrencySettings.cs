namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Models
{
    /// <summary>
    /// Класс для работы с ответом currencyApi, содержащий текущие настройки приложения
    /// </summary>
    public class CurrencySettings
    {
        public string? defaultCurrency { get; set; }
        public string? baseCurrency { get; set; }
        public int requestLimit { get; set; }
        public int requestCount { get; set; }
        public int currencyRoundCount { get; set; }
    }
}
