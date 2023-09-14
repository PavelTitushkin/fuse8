namespace InternalApi.Models.ModelResponse
{
    /// <summary>
    /// Класс курса валют 
    /// </summary>
    public class CurrencyWithDate
    {
        /// <summary>
        /// Дата курса валют
        /// </summary>
        public string? Date { get; set; }
        /// <summary>
        /// Код валюты
        /// </summary>
        public string? Code { get; set; }
        /// <summary>
        /// Значение валюты
        /// </summary>
        public decimal? Value { get; set; }
    }
}
