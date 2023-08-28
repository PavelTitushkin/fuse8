using System.Text.Json.Serialization;

namespace InternalApi.Models.ModelDTO
{
    /// <summary>
    /// Курс валюты
    /// </summary>
    /// <param name="CurrencyType">Валюта</param>
    /// <param name="Value">Значение курса</param>
    public class CurrencyDTO
    {
        [JsonPropertyName("CurrencyType")]
        public CurrencyType CurrencyType { get; set; }

        [JsonPropertyName("Value")]
        public decimal Value { get; set; }
    }

    public enum CurrencyType
    {
        USD, EUR, RUB, KZT, BYN
    }
}
