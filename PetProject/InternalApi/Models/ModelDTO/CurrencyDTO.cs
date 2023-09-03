namespace InternalApi.Models.ModelDTO
{
    /// <summary>
    /// Курс валюты
    /// </summary>
    public class CurrencyDTO
    {
        /// <summary>
        /// <inheritdoc cref="CurrencyType"/>
        /// </summary>
        public CurrencyType CurrencyType { get; set; }

        /// <summary>
        /// Значение валюты
        /// </summary>
        public decimal Value { get; set; }
    }

    /// <summary>
    /// Код валюты
    /// </summary>
    public enum CurrencyType
    {
        /// <summary>
        /// USD-Американский доллар
        /// </summary>
        USD,
        /// <summary>
        /// EUR - Евро
        /// </summary>
        EUR,
        /// <summary>
        /// RUB - Российский рубль
        /// </summary>
        RUB,
        /// <summary>
        /// KZT - Казахстанский тенге
        /// </summary>
        KZT,
        /// <summary>
        /// BYN - Белорусский рубль
        /// </summary>
        BYN
    }
}
