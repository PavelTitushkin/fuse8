using InternalApi.Models.ModelResponse;

namespace Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelDTO
{
    /// <summary>
    /// Класс курсов валют
    /// </summary>
    public class CurrenciesDTO
    {
        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Дата курсов валют
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Список курсов валют
        /// </summary>
        public List<Currency> CurrenciesList { get; set; }
    }
}
