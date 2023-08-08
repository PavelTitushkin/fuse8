using System.Text.Json.Serialization;

namespace Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelResponse
{
    /// <summary>
    /// Класс для работы с ответом currencyApi 
    /// </summary>
    public class SettingsResponse
    {
        /// <summary>
        /// Аккаунт id
        /// </summary>
        [JsonPropertyName("account_id")]
        public long AccountId { get; set; }

        [JsonPropertyName("quotas")]
        public Quotas Quotas { get; set; }
    }

    /// <summary>
    /// Лимит запросов
    /// </summary>
    public class Quotas
    {
        [JsonPropertyName("month")]
        public Month Month { get; set; }

        [JsonPropertyName("grace")]
        public Grace Grace { get; set; }
    }

    /// <summary>
    /// Лимит запрос в месяц
    /// </summary>
    public class Month
    {
        /// <summary>
        /// Общее количество запросов
        /// </summary>
        [JsonPropertyName("total")]
        public int Total { get; set; }

        /// <summary>
        /// Использованное количество запросов
        /// </summary>
        [JsonPropertyName("used")]
        public int Used { get; set; }

        /// <summary>
        /// Оставшейся количество запросов
        /// </summary>
        [JsonPropertyName("remaining")]
        public int Remaining { get; set; }
    }
    public class Grace
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("used")]
        public int Used { get; set; }

        [JsonPropertyName("remaining")]
        public int Remaining { get; set; }
    }
}
