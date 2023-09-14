namespace InternalApi.Models.ModelResponse
{
    /// <summary>
    /// Класс курса валют 
    /// </summary>
    public class Currency
    {
        /// <summary>
        /// Код валюты
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Значение валюты
        /// </summary>
        public decimal Value { get; set; }
    }
}
