namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Models
{
    /// <summary>
    /// Класс для работы с ответом currencyApi
    /// </summary>
    public class CurrencyRateResponse
    {
        public Meta meta { get; set; }
        public Dictionary<string, CodeValue> data { get; set; }
    }

    /// <summary>
    /// Класс для работы с ответом currencyApi
    /// </summary>
    public class Meta
    {
        /// <summary>
        /// Дата курса валют
        /// </summary>
        public DateTime last_updated_at { get; set; }
    }

    /// <summary>
    /// Класс для работы с ответом currencyApi
    /// </summary>
    public class CodeValue
    {
        /// <summary>
        /// Код валюты
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// значение валюты
        /// </summary>
        public decimal value { get; set; }
    }
}
