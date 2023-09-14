namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Models.ModelResponse
{
    /// <summary>
    /// Настройки Api
    /// </summary>
    public class ApiSettings
    {
        /// <summary>
        /// Код валюты по умолчанию
        /// </summary>
        public string DefaultCurrency { get; set; }

        /// <summary>
        /// Код базовой валюты 
        /// </summary>
        public string BaseCurrency { get; set; }

        /// <summary>
        /// Превышен ли лимит запросов 
        /// </summary>
        public bool NewRequestsAvailable { get; set; }

        /// <summary>
        /// Значение количества знаков после запятой
        /// </summary>
        public int CurrencyRoundCount { get; set; }
    }
}
