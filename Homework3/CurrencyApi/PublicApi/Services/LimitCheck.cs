using currencyapi;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Abstractions;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Exceptions;
using Newtonsoft.Json.Linq;

namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Services
{
    public class LimitCheck : ILimitCheck
    {
        public bool CheckLimit()
        {
            var fx = new Currencyapi("[YOUR_API_KEY]");
            var status = fx.Status();
            var dataStatus = JObject.Parse(status);
            int.TryParse(Convert.ToString(dataStatus["quotas"]["month"]["used"]), out int requestLimit);

            if (requestLimit <= 0)
                return true;

            return false;
        }
    }
}
