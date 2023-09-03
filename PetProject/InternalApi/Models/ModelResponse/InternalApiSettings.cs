namespace InternalApi.Models.ModelResponse
{
    /// <summary>
    /// Класс значения настроек внешнего Api
    /// </summary>
    public class InternalApiSettings
    {
        /// <summary>
        /// Код валюты
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Лимит запросов к внешнему Api
        /// </summary>
        public bool Limit { get; set; }
    }
}
