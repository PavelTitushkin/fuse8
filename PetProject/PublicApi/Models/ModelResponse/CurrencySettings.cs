namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Models.ModelResponse
{
    /// <summary>
    /// Класс для работы с ответом currencyApi, содержащий текущие настройки приложения
    /// </summary>
    public class CurrencySettings
    {
        /// <summary>
        /// Код валюты по умолчанию
        /// </summary>
        public string? DefaultCurrency { get; set; }

        /// <summary>
        /// Код базовой валюты 
        /// </summary>
        public string? BaseCurrency { get; set; }

        /// <summary>
        /// Лимит запросов
        /// </summary>
        public int RequestLimit { get; set; }

        /// <summary>
        /// Количество запросов
        /// </summary>
        public int RequestCount { get; set; }

        /// <summary>
        /// Значение количества знаков после запятой
        /// </summary>
        public int CurrencyRoundCount { get; set; }
    }
}
