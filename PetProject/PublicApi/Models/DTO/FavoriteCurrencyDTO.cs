namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Models.DTO
{
    /// <summary>
    /// Класс Избранной валюты
    /// </summary>
    public class FavoriteCurrencyDTO
    {
        /// <summary>
        /// Название избанной валюты
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Код Избранной валюты
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Код базовой валюты
        /// </summary>
        public string BaseCurrency { get; set; }
    }
}
