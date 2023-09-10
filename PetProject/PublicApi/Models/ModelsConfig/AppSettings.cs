namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Models.ModelsConfig
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
    }
}
