namespace InternalApi.Models.ModelsConfig
{
    /// <summary>
    /// Класс кофигурации приложения
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Код валюты по умолчанию
        /// </summary>
        public string Default { get; set; }

        /// <summary>
        /// Код базовой валюты 
        /// </summary>
        public string Base { get; set; }

        /// <summary>
        /// Значение количества знаков после запятой
        /// </summary>
        public int Round { get; set; }

        /// <summary>
        /// Ключ к внешнему Api
        /// </summary>
        public string APIKey { get; set; }

        /// <summary>
        /// Базовый путь к внешнему Api
        /// </summary>
        public string BasePath { get; set; }

        /// <summary>
        /// Путь к кэш-файлу на диске
        /// </summary>
        public string PathFile { get; set; }

        /// <summary>
        /// Время актуальности кэша
        /// </summary>
        public int CacheLifetime { get; set; }

        /// <summary>
        /// Время ожидания ответа
        /// </summary>
        public int WaitTimeTaskExecution { get; set; }
    }
}
