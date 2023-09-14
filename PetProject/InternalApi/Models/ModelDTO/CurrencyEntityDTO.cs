namespace Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelDTO
{
    /// <summary>
    /// Класс курс валют
    /// </summary>
    public class CurrencyEntityDTO
    {
        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        public int Id { get; set; }

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
