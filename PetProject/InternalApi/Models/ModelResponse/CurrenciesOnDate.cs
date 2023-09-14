namespace InternalApi.Models.ModelResponse
{
    /// <summary>
    /// Класс курсов валют на определённую дату
    /// </summary>
    public class CurrenciesOnDate
    {
        /// <summary>
        /// Дата
        /// </summary>
        public DateTime LastUpdatedAt { get; set; }

        /// <summary>
        /// Курсы валют
        /// </summary>
        public Currency[] Currencies { get; set; }
    }
}
