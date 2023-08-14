using System.Text.Json.Serialization;

namespace InternalApi.Models.ModelResponse
{
    /// <summary>
    /// Класс для работы с ответом currencyApi
    /// </summary>
    public class CurrencyRateResponse
    {
        [JsonPropertyName("meta")]
        public Meta Meta { get; set; }

        [JsonPropertyName("data")]
        public Dictionary<string, CodeValue> Data { get; set; }
    }

    /// <summary>
    /// Класс для работы с ответом currencyApi
    /// </summary>
    public class Meta
    {
        /// <summary>
        /// Дата курса валют
        /// </summary>
        [JsonPropertyName("last_updated_at")]
        public DateTime LastUpdatedAt { get; set; }
    }

    /// <summary>
    /// Класс для работы с ответом currencyApi
    /// </summary>
    public class CodeValue
    {
        /// <summary>
        /// Код валюты
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; }
        /// <summary>
        /// значение валюты
        /// </summary>
        [JsonPropertyName("value")]
        public decimal Value { get; set; }
    }
}
