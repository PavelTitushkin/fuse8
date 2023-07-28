using currencyapi;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Abstractions;
using Newtonsoft.Json.Linq;

namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Services
{
    /// <summary>
    /// Сервис для работы с currencyApi
    /// </summary>
    public class LimitCheckService : ILimitCheckService
    {
        private readonly IConfiguration _configuration;

        public LimitCheckService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool CheckLimit()
        {
            var apiKey = _configuration["Settings:APIKey"];
            var fx = new Currencyapi(apiKey);
            var status = fx.Status();
            var dataStatus = JObject.Parse(status);
            int.TryParse(Convert.ToString(dataStatus["quotas"]["month"]["used"]), out int requestLimit);

            if (requestLimit <= 0)
                return true;

            return false;
        }
    }
}
