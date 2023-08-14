namespace InternalApi.Models.ModelResponse
{
    /// <summary>
    /// Класс для работы с ответом currencyApi, содержащий текущие настройки приложения
    /// </summary>
    public class CurrencySettings
    {
        public string? DefaultCurrency { get; set; }
        public string? BaseCurrency { get; set; }
        public int RequestLimit { get; set; }
        public int RequestCount { get; set; }
        public int CurrencyRoundCount { get; set; }
    }
}
